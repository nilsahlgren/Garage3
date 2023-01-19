using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage3.Data;
using Garage3.Models;
using System.Globalization;
using Microsoft.VisualBasic;

namespace Garage3.Controllers
{
    public class MembersController : Controller
    {
        private readonly Garage3Context _context;

        public MembersController(Garage3Context context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return _context.Member != null ?
                        View(await _context.Member.ToListAsync()) :
                        Problem("Entity set 'Garage3Context.Member' is null.");
        }

        // Members Overview with search for PersNo and LastName
        public async Task<IActionResult> MemberOverview(string persNo, string lastName)
        {
            var members = from mem in _context.Member select mem;
            if (!String.IsNullOrEmpty(persNo))
            {
                members = members.Where(mem => mem.PersNo.Contains(persNo));
            }

            if (!String.IsNullOrEmpty(lastName))
            {
                members = members.Where(mem => mem.LastName.Contains(lastName));
            }
            return View(await members
                .Include(veh => veh.Vehicles)
                .OrderBy(name => name.FirstName.Substring(0, 2))
                .ToListAsync());
        }

        // List of individual membe's vehicles
        public async Task<IActionResult> MemberVehicleList(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }
            var member = await _context.Member
                .FirstOrDefaultAsync(mem => mem.Id == id);

            if (member == null)
            {
                return NotFound();
            }
            var vehicles = _context.Vehicle
                .Include(veh => veh.Session);
            var memberVehicles = vehicles
                .Where(veh => veh.MemberId == id)
                .ToList();
            return View(memberVehicles);
        }


        public async Task<IActionResult> SelectMemberForCheckin()
        {

            int ageLimit = int
                   .Parse(DateTime.Today
                   .AddYears(-18)
                   .ToString()
                   .Replace("-", string.Empty)
                   .Substring(0, 8));

            var allMembers = await _context.Member.ToListAsync();
            var membersOfAge = new List<Member>();

            foreach (var member in allMembers)
            {
                int ageCandidate = int
                    .Parse(member.PersNo
                    .Replace("-", string.Empty)
                    .Substring(0, 8));

                if (ageCandidate <= ageLimit)
                {
                    membersOfAge.Add(member);
                }
            }

            return membersOfAge != null ?
                        View(membersOfAge.ToList()) :
                        Problem("Entity set 'Garage3Context.Member'  is null.");
        }

        public async Task<IActionResult> SelectMemberForRegistration()
        {

            return _context.Member != null ?
                        View(await _context.Member.ToListAsync()) :
                        Problem("Entity set 'Garage3Context.Member'  is null.");
        }

        public async Task<IActionResult> SelectVehicleForCheckIn(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }

            var vehicles = _context.Vehicle.Include(v => v.Session);
            var unparkedVehicles = vehicles.Where(v => v.Session == null);
            var memberUnparkedVehicles = unparkedVehicles.Where(v => v.MemberId == id).ToList();

            return View(memberUnparkedVehicles);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersNo,FirstName,LastName")] Member member)
        {
            if (member.FirstName == member.LastName)
            {
                ViewData["DoubleName"] = member.FirstName;
                return View(member);
            }
            if (ModelState.IsValid)
            {
                var allMembers = _context.Member;

                foreach (var memberExist in allMembers)
                {
                    if (memberExist.PersNo.Contains(member.PersNo))
                    {
                        ViewData["MemberAlreadyExists"] = member.PersNo;
                        return View(member);
                    }
                }

                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersNo,FirstName,LastName")] Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Member == null)
            {
                return Problem("Entity set 'Garage3Context.Member'  is null.");
            }
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return (_context.Member?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
