using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Api.Models;

namespace Inventory.Api.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public enum UserRole
		{
			Admin,
			Vendor,
		}

		public UserRole Role { get; set; } // User role (e.g., Admin, User)
		public DateTime CreatedAt { get; set; } // Date and time when the user was created
		public DateTime UpdatedAt { get; set; } // Date and time when the user was last updated


	}
}
