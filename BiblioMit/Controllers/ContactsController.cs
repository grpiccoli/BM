using BiblioMit.Authorization;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<AppUser> _userManager;

        public ContactsController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            IQueryCollection q = Request.Query;

            var contacts = from c in _context.Contact
                           .Include(c => c.Centre)
                            .ThenInclude(c => c.Company)
                           .Include(c => c.Centre.Coordinates)
                           .Include(c => c.Centre.Comuna)
                           select c;
            var comuna = _context.Comuna
                .Include(c => c.Provincia)
                    .ThenInclude(c => c.Region);
            ViewData["comunas"] = comuna;
            string[] temp = q["c"];
            ViewData["c"] = temp;
            if (q["c"].Count() > 0)
            {
                contacts = contacts.Where(c => q["c"].ToString().Contains(Convert.ToString(c.Centre.ComunaId)));
            }

            var isAuthorized = User.IsInRole("Administrador") && 
                               User.HasClaim("Contactos","Contactos");

            var currentUserId = _userManager.GetUserId(User);

            // Only approved contacts are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                contacts = contacts.Where(c => c.Status  == ContactStatus.Aprobado
                                            || c.OwnerId == currentUserId);
            }

            return View(await contacts.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .SingleOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorizedRead = await _authorizationService.AuthorizeAsync(
                                                       User, contact,
                                                       ContactOperations.Read);

            var isAuthorizedApprove = await _authorizationService.AuthorizeAsync(
                                           User, contact,
                                           ContactOperations.Approve);

            if (contact.Status != ContactStatus.Aprobado &&   // Not approved.
                                  !isAuthorizedRead.Succeeded &&        // Don't own it.
                                  !isAuthorizedApprove.Succeeded)       // Not a manager.
            {
                return new ChallengeResult();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize(Roles = "Administrador",Policy = "Contactos")]
        public IActionResult Create()
        {
            //return View();
            // TODO-Rick - remove, this is just for quick testing.
            return View(new ContactEditViewModel
            {
                Last = "Apellido",
                Email = _userManager.GetUserName(User),
                Name = "Nombre",
                Position = Position.Direccion,
                Description = "Descripción",
                Phone = 56912345678,
                OpenHr = Convert.ToDateTime("9:00"),
				CloseHr = Convert.ToDateTime("18:00")
            });
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador",Policy ="Contactos")]
        public async Task<IActionResult> Create(ContactEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            var contact = ViewModel_to_model(new Contact(), editModel);

            contact.OwnerId = _userManager.GetUserId(User);

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                                                        User, contact,
                                                        ContactOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            _context.Add(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Contacts/Edit/5
        [Authorize(Roles = "Administrador,Editor",Policy = "Contactos")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact.SingleOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                                                        User, contact,
                                                        ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            var editModel = Model_to_viewModel(contact);

            return View(editModel);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Editor",Policy = "Contactos")]
        public async Task<IActionResult> Edit(int id, ContactEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            // Fetch Contact from DB to get OwnerId.
            var contact = await _context.Contact.SingleOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, contact,
                                                                ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            contact = ViewModel_to_model(contact, editModel);

            if (contact.Status == ContactStatus.Aprobado)
            {
                // If the contact is updated after approval, 
                // and the user cannot approve set the status back to submitted
                var canApprove = await _authorizationService.AuthorizeAsync(User, contact,
                                        ContactOperations.Approve);

                if (!canApprove.Succeeded) contact.Status = ContactStatus.Ingresado;
            }

            _context.Update(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Contacts/Delete/5
        [Authorize(Roles = "Administrador",Policy = "Contactos")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .SingleOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, contact,
                                        ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador",Policy = "Contactos")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contact.SingleOrDefaultAsync(m => m.ContactId == id);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, contact,
                                        ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador",Policy = "Contactos")]
        public async Task<IActionResult> SetStatus(int id, ContactStatus status)
        {
            var contact = await _context.Contact.SingleOrDefaultAsync(m => m.ContactId == id);

            var contactOperation = (status == ContactStatus.Aprobado) ? ContactOperations.Approve
                                                                      : ContactOperations.Reject;

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, contact,
                                        contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            contact.Status = status;
            _context.Contact.Update(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.ContactId == id);
        }

        private Contact ViewModel_to_model(Contact contact, ContactEditViewModel editModel)
        {
            contact.Last = editModel.Last;
            contact.Email = editModel.Email;
            contact.Name = editModel.Name;
            contact.Position = editModel.Position;
            contact.Description = editModel.Description;
            contact.Phone = editModel.Phone;
            contact.OpenHr = editModel.OpenHr;
            contact.CloseHr = editModel.CloseHr;

            return contact;
        }

        [Authorize(Roles = "Administrador",Policy = "Contactos")]
        private ContactEditViewModel Model_to_viewModel(Contact contact)
        {
            var editModel = new ContactEditViewModel()
            {
                ContactId = contact.ContactId,
                Last = contact.Last,
                Email = contact.Email,
                Name = contact.Name,
                Position = contact.Position,
                Description = contact.Description,
                Phone = contact.Phone,
                OpenHr = contact.OpenHr,
                CloseHr = contact.CloseHr
            };

            return editModel;
        }
    }
}
