using Inventory_Management.Context;
using Inventory_Management.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Management.Managers
{
    public class UserManager
    {
        private readonly InventoryDbContext _context;

        public UserManager(InventoryDbContext context)
        {
            _context = context;
        }

        // Add a new user
        public async Task<User> AddUserAsync(User user, User.UserRole role)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                if (string.IsNullOrWhiteSpace(user.Username))
                {
                    throw new ArgumentException("Username cannot be empty", nameof(user.Username));
                }

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    throw new ArgumentException("Password cannot be empty", nameof(user.Password));
                }

                // Check if username already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"Username '{user.Username}' is already taken");
                }

                // Hash password and set user metadata
                HashPassword(user);
                user.Role = role;
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while adding user: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                       ex is not ArgumentException &&
                                       ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error adding user: {ex.Message}", ex);
            }
        }

        // Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .ToListAsync();

                // Remove sensitive data
                foreach (var user in users)
                {
                    user.Password = null; // Don't return password hashes
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving users: {ex.Message}", ex);
            }
        }

        // Get a user by ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(id));
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                // Remove sensitive data
                user.Password = null; // Don't return password hash

                return user;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving user with ID {id}: {ex.Message}", ex);
            }
        }

        // Get a user by username
        public async Task<User> GetByUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new ArgumentException("Username cannot be empty", nameof(username));
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with username '{username}' not found");
                }

                return user;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving user by username '{username}': {ex.Message}", ex);
            }
        }

        // Update a user
        public async Task UpdateUserAsync(int id, User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(user.Id));
                }

                // Verify user exists
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    throw new InvalidOperationException($"User with ID {user.Id} not found");
                }

                // Check if username is being changed and if new username is already taken
                if (user.Username != existingUser.Username)
                {
                    var usernameExists = await _context.Users.AnyAsync(u => u.Username == user.Username && u.Id != user.Id);
                    if (usernameExists)
                    {
                        throw new InvalidOperationException($"Username '{user.Username}' is already taken");
                    }
                }

                existingUser.Password = user.Password; // Update password if provided
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user); // Update the user entity
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating user: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                       ex is not ArgumentException &&
                                       ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error updating user with ID {user?.Id}: {ex.Message}", ex);
            }
        }

        // Delete a user
        public async Task DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than zero", nameof(id));
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }
                /*
                // Check if user has related inventory transactions
                if (user.InventoryTransactions != null && user.InventoryTransactions.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot delete user with ID {id} because they have {user.InventoryTransactions.Count} related inventory transactions");
                }
                */
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while deleting user: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error deleting user with ID {id}: {ex.Message}", ex);
            }
        }

        // Hash password
        public void HashPassword(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("Password cannot be empty when hashing", nameof(user.Password));
            }

            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error hashing password", ex);
            }
        }

        // Verify password
        public bool VerifyPassword(User user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("User has no stored password hash", nameof(user.Password));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty when verifying", nameof(password));
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error verifying password", ex);
            }
        }
    }
}
