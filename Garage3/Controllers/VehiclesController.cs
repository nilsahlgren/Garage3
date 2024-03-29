﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage3.Data;
using Garage3.Models;

namespace Garage3.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Garage3Context _context;

        public VehiclesController(Garage3Context context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("created"))
            {
                ViewData["ShowElement"] = TempData["created"];
            }
            if (TempData.ContainsKey("checkedout"))
            {
                ViewData["ShowElement"] = TempData["checkedout"];
            }
            if (TempData.ContainsKey("updated"))
            {
                ViewData["ShowElement"] = TempData["updated"];
            }
            return _context.Vehicle != null ?
                        View(await _context.Vehicle.ToListAsync()) :
                        Problem("Entity set 'Garage3Context.Vehicle'  is null.");
        }



        
        public async Task<IActionResult> Overview(string regNo, string vehicleTypeName)
        {
            var sessions = _context.Session.Include(s => s.ParkingSpaces).Where(s => s.VehicleId != null);

            List<VehicleViewModel> vehicleVMList = new List<VehicleViewModel>();

            foreach (Session s in sessions)
            {
                var member = _context.Member.FirstOrDefault(m => m.Id == s.MemberId);
                var vehicle = _context.Vehicle.FirstOrDefault(v => v.Id == s.VehicleId);
                VehicleViewModel vehicleVM = new VehicleViewModel();
                vehicleVM.VehicleTypeName = vehicle.VehicleTypeName;
                vehicleVM.RegNo = vehicle.RegNo;
                vehicleVM.ParkTime = DateTime.Now - s.TimeOfArrival;
                vehicleVM.Owner = member.FirstName + " " + member.LastName;
                //MembershipLevel = member.MembershipLevel;
                string parkingSpaces = "";
                foreach (ParkingSpace p in s.ParkingSpaces)
                {
                    parkingSpaces += p.Id + ", ";
                }
                parkingSpaces = parkingSpaces.Substring(0, parkingSpaces.Length);
                vehicleVM.ParkingSpaces = parkingSpaces;

                vehicleVMList.Add(vehicleVM);
            }

            if (!String.IsNullOrEmpty(regNo))
            {
                vehicleVMList = vehicleVMList.Where(v => v.RegNo.Contains(regNo)).ToList();
            }

            if (!String.IsNullOrEmpty(vehicleTypeName))
            {
                vehicleVMList = vehicleVMList.Where(v => v.VehicleTypeName.Contains(vehicleTypeName)).ToList();
            }

            return View(vehicleVMList);
        }

        public async Task<IActionResult> SelectVehicleForCheckout()
        {
            var allVehicles = _context.Vehicle
                .Include(v => v.Session);
            var parkedVehicles = allVehicles
                .OrderBy(v => v.RegNo)
                .Where(v => v.Session.TimeOfDeparture
                .ToString()
                .StartsWith("000"));


            //Note: If one sets vehicle.Session to null (In the SessionsController...), the session.VehicleId
            //is set to null in the Session table in the database (Yes, it took quite some investigation to find this out...)
            //In order to keep the sessions table intact its neccesary to instead look at the value
            //for session.TimeOfDeparture to get the ACTIVE Sessions.
            //var parkedVehicles = allVehicles.Where(v => v.Session != null);
            return View(await parkedVehicles.ToListAsync());
        }

        public async Task<IActionResult> ConfirmCheckout(int? id)
        {
            var allVehicles = _context.Vehicle.Include(v => v.Session);
            var selectedVehicle = await allVehicles.FirstOrDefaultAsync(v => v.Id == id);
            var vehicleSize = await _context.VehicleType.FirstOrDefaultAsync(vs => vs.Name == selectedVehicle.VehicleTypeName);

            //NOTE: this i NULL!
            // var parkingSpaces = selectedVehicle.Session.ParkingSpaces; 
            //NO parking spaces are added to Vehicle.Session.

            ViewData["ConfirmedCheckedOutVehicleId"] = selectedVehicle?.Id;
            TempData["ConfirmedCheckedOutVehicleSize"] = vehicleSize.Size;

            return View(selectedVehicle);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewBag.VehicleTypeList = _context.VehicleType.ToList();
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberId,RegNo,VehicleTypeName,Brand,Model,Color,NoOfWheels")] Vehicle vehicle)
        {
            if (ModelState.IsValid)

            {
                //var allVehicles = _context.Vehicle;

                //foreach (var regNoExist in allVehicles)
                //{
                //    if (regNoExist.RegNo == vehicle.RegNo)
                //    {
                //        ViewData["RegNoAlreadyExists"] = vehicle.RegNo;
                //        return View(vehicle);
                //    }
                //}

                vehicle.MemberId = int.Parse(TempData["MemberIdData"].ToString());
                _context.Add(vehicle);
                var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == vehicle.MemberId);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Overview));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,RegNo, VehicleTypeName,Brand,Model,Color,NoOfWheels")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicle == null)
            {
                return Problem("Entity set 'Garage3Context.Vehicle'  is null.");
            }
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return (_context.Vehicle?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
