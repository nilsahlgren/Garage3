using System;
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
    public class SessionsController : Controller
    {
        private readonly Garage3Context _context;

        public SessionsController(Garage3Context context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
              return _context.Session != null ? 
                          View(await _context.Session.ToListAsync()) :
                          Problem("Entity set 'Garage3Context.Session'  is null.");
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberId,VehicleId,TimeOfArrival,TimeOfDeparture,Price")] Session session)
        {
            session.MemberId = int.Parse(TempData["MemberIdData"].ToString());
            session.VehicleId = int.Parse(TempData["VehicleIdData"].ToString());
            session.TimeOfArrival = DateTime.Now;
            var vehicle = await _context.Vehicle
            .FirstOrDefaultAsync(v => v.Id == session.VehicleId);
            var vehicleType = await _context.VehicleType.FirstOrDefaultAsync(t => t.Name == vehicle.VehicleTypeName);
            int firstFreeSpaceId = ParkingSpaceFinder(vehicleType.Size);

            if (firstFreeSpaceId < 999)
            {
                session.ParkingSpaces = new List<ParkingSpace>();
                for (var i = 0; i < vehicleType.Size; i++)
                {
                    ParkingSpace chosenSpace = await _context.ParkingSpace.FirstOrDefaultAsync(p => p.Id == (firstFreeSpaceId+i));
                    chosenSpace.SessionId = session.Id;
                    session.ParkingSpaces.Add(chosenSpace);
                }

                if (ModelState.IsValid)
                {
                    _context.Add(session);
                    vehicle.Session = session;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(session);
        }

        public int ParkingSpaceFinder(int size)
        {
            int firstFreeSpaceId = 999;
            var activeSessions = _context.Session.Where(s => s.TimeOfDeparture < DateTime.Now).ToList();
            int numberOfSpaces = _context.ParkingSpace.ToList().Count;
            string[] spaceStatus = new string[numberOfSpaces+1];
            int spaceStatusCount = spaceStatus.Length;
            if (activeSessions != null)
            {
                    for (var j = 0; j < activeSessions.Count; j++)
                    {
                        foreach (ParkingSpace p in activeSessions[j].ParkingSpaces)
                        {
                           spaceStatus[p.Id] = "taken";
                        }
                    }
            }

            for (var i = 1; i <= numberOfSpaces; i++)
            {
                var freeAdjacent = 0;
                for (var j = 0; j < size; j++)
                {
                    if (i + j <= numberOfSpaces)
                    {
                        if (spaceStatus[i + j] != "taken")
                        {
                            freeAdjacent++;
                        }
                    }
                }
                if (size <= freeAdjacent)
                {
                    firstFreeSpaceId = i;
                    break;
                }
            }

            return firstFreeSpaceId;
        }


// GET: Sessions/Edit/5
public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }

            var session = await _context.Session.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,VehicleId,TimeOfArrival,TimeOfDeparture,Price")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
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
            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Session == null)
            {
                return Problem("Entity set 'Garage3Context.Session'  is null.");
            }
            var session = await _context.Session.FindAsync(id);
            if (session != null)
            {
                _context.Session.Remove(session);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
          return (_context.Session?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
