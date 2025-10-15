using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.DTO.LocationDTOs;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ApplicationDbContext context, ILogger<LocationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET api/location
        //Get all locations
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Locations>>> GetAllLocations()
        {
            try
            {
                var location = await _context.Locations.ToListAsync();
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get all locations");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("/")]
        public async Task<IActionResult> Create_Location(
            LocationCreateDTO createLocation
        )
        {
            try
            {
                var newLocation = new Locations
                {
                    Name = createLocation.Name,
                    Address_Primary = createLocation.Address_One,
                    Address_Secondary = createLocation.Address_Two,
                    City = createLocation.City,
                    Province = createLocation.Province,
                    Postal_Code = createLocation.Postal_Code,
                    Phone_Number = createLocation.Phone_Number
                };
                _context.Locations.Add(newLocation);
                await _context.SaveChangesAsync();
                return Ok("Location was created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpGet("/{location_id}")]
        public async Task<IActionResult> get_locations(
            int location_id
        )
        {
            try
            {
                var location = await _context.Locations.Where(l => l.Location_Id == location_id).FirstOrDefaultAsync();
                if (location == null)
                {
                    return NotFound("Location was not Found");
                }
                return Ok(location);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [HttpPut("/{location_id}")]
        public async Task<IActionResult> Update_Location(
            int location_id,
            LocationUpdateDTO location_update
        )
        {
            try
            {
                var location = await _context.Locations.Where(l => l.Location_Id == location_id).FirstOrDefaultAsync();
                if (location == null)
                {
                    return NotFound("Location to be updated was not found.");
                }
                if (!string.IsNullOrEmpty(location_update.Name))
                {
                    location.Name = location_update.Name;
                }
                if (!string.IsNullOrEmpty(location_update.Address_One))
                {
                    location.Address_Primary = location_update.Address_One;
                }
                if (!string.IsNullOrEmpty(location_update.Address_Two))
                {
                    location.Address_Secondary = location_update.Address_Two;
                }
                if (!string.IsNullOrEmpty(location_update.City))
                {
                    location.City = location_update.City;
                }
                if (!string.IsNullOrEmpty(location_update.Province))
                {
                    location.Province = location_update.Province;
                }
                if (!string.IsNullOrEmpty(location_update.Postal_Code))
                {
                    location.Postal_Code = location_update.Postal_Code;
                }
                if (!string.IsNullOrEmpty(location_update.Phone_Number))
                {
                    location.Phone_Number = location_update.Phone_Number;
                }

                _context.Update(location);
                await _context.SaveChangesAsync();
                return Ok(location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpDelete("/{location_id}")]
        public async Task<IActionResult> Delete_Location(
            int location_id
        )
        {
            try
            {
                var deleteLocation = await _context.Locations.Where(l => l.Location_Id == location_id).FirstOrDefaultAsync();
                if (deleteLocation == null)
                {
                    return NotFound("Location was not found to delete");
                }
                _context.Remove(deleteLocation);
                await _context.SaveChangesAsync();
                return Ok("Location was Deleted from the system.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [HttpPost("/{location_id}/menus")]
        public async Task<IActionResult> Add_Menu_To_Location(
            int location_id,
            MenuLocationCreateDTO menuLocationCreate
        )
        {
            try
            {
                var location = _context.Locations.Where(l => l.Location_Id == location_id).FirstOrDefault();
                if (location == null)
                {
                    return NotFound("Location was not found");
                }
                _context.MenuLocations.Add(new MenuLocations
                {
                    Location_Id = location_id,
                    Menu_Id = menuLocationCreate.Menu_id
                });
                await _context.SaveChangesAsync();
                return Ok("Menu Location was Added Successfully ");


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }


        }
        [HttpDelete("/{location_id}/menus/{menu_id}")]
        public async Task<IActionResult> Remove_Menu_From_Location(
            int location_id,
            int menu_id
        )
        {
            try
            {
                var menuDeletion = await  _context.MenuLocations.Where(ml => ml.Location_Id == location_id && ml.Menu_Id == menu_id).FirstOrDefaultAsync();
                if (menuDeletion == null)
                {
                    return NotFound("Menu Location was not found.");
                }
                _context.Remove(menuDeletion);
                await _context.SaveChangesAsync();
                return Ok("Menu was Deleted From the location.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpGet("/{location_id}/menus")]
        public async Task<IActionResult> list_location_menus(
            int location_id
        )
        {
            try
            {
                var location = await _context.Locations.Where(l => l.Location_Id == location_id).FirstOrDefaultAsync();
                if (location == null)
                {
                    return NotFound("Location was not found.");
                }
                var menuIds = await _context.MenuLocations
                    .Where(ml => ml.Location_Id == location_id)
                    .Select(ml => ml.Menu_Id)  // Only select the Menu_Id
                    .ToListAsync();

                return Ok(menuIds);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

    }
}