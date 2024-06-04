using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IO.Ably;
using IO.Ably.Realtime;
using System;
using System.Linq;
using System.Threading.Tasks;
using kidzkonnect2._0.Models;

namespace kidzkonnect2._0.Controllers
{
    public class MessagesController : Controller
    {
        private readonly AblyRealtime _ably;
        private readonly kidzkonnectContext _context;

        public MessagesController(kidzkonnectContext context)
        {
            _context = context;
            _ably = new AblyRealtime("u7sjCg.kolkvQ:mA9tLK7D27Pejjtr2HSAyAnENtCuKYHK48RwCtI-H2c");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int? idDemande, string message)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            if (idDemande == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.IdDemande == idDemande);
            if (connection == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var activeProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(p => p.IdUtilisateur == userId);
            if (activeProfile == null)
            {
                return RedirectToAction("Error", "Home");
            }

            if (activeProfile.IdProfil != connection.IdProfilEnvoyeur &&
                activeProfile.IdProfil != connection.IdProfilReceveur)
            {
                return RedirectToAction("Error", "Home");
            }

            int receiverProfileId;
            if (activeProfile.IdProfil == connection.IdProfilEnvoyeur)
            {
                receiverProfileId = connection.IdProfilReceveur ?? 0;
            }
            else
            {
                receiverProfileId = connection.IdProfilEnvoyeur ?? 0;
            }

            var channelName = $"kk_{idDemande}";
            var channel = _ably.Channels.Get(channelName);

            var messagePayload = new
            {
                senderProfileId = activeProfile.IdProfil,
                receiverProfileId = receiverProfileId,
                message = message,
                persisted = true
            };

            // Publish the message to the correct channel based on idDemande
            channel.Publish("message", messagePayload);

            return RedirectToAction("Chat", new { idDemande = idDemande });
        }

        [HttpGet("Chat/{idDemande}")]
        public async Task<IActionResult> Chat(int? idDemande)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            if (idDemande == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.IdDemande == idDemande);
            if (connection == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            var childProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(p => (p.IdProfil == connection.IdProfilEnvoyeur || p.IdProfil == connection.IdProfilReceveur) && p.IdUtilisateur == userId);

            if (childProfile == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var parentUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.IdUtilisateur == userId);

            var parentProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(p => p.IdUtilisateur == userId);

            if (parentProfile == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var connectionExists = await _context.Connections.AnyAsync(c => c.IdProfilEnvoyeur == childProfile.IdProfil || c.IdProfilReceveur == childProfile.IdProfil);

            if (!connectionExists)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            var channelName = $"kk_{idDemande}";
            var channel = _ably.Channels.Get(channelName);

            // Subscribe to the correct channel based on idDemande
            channel.Subscribe(message =>
            {
                // Handle incoming messages here if needed
            });

            // Fetch messages from the correct Ably channel
            var messages = await channel.HistoryAsync();

            // Pass relevant data to the view
            ViewData["SenderProfileId"] = childProfile.IdProfil;
            ViewData["ReceiverProfileId"] = parentProfile.IdProfil;
            ViewBag.ChildProfile = childProfile;
            ViewBag.ParentUser = parentUser;
            ViewBag.Messages = messages.Items; // Assuming Ably returns messages as Items

            return View();
        }


        public async Task<IActionResult> Disconnect()
        {
            // Close Ably connection
            _ably.Connection.Close();

            return Ok(); // or return any appropriate response
        }

		public IActionResult ChatList()
		{
			return View(); // or return any appropriate response
		}
	}
}
