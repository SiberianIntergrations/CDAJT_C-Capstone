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

        /// <summary>
        /// Retrieves all locations from the database.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="Locations"/> objects.
        /// Returns HTTP 200 (OK) with the list of all locations on success.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the list of all locations</response>
        /// <response code="500">If an internal error occurs while retrieving locations</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/location
        ///
        /// Returns all locations in the system.
        /// </remarks>
        //GET api/location
        //Get all locations
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Locations>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


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

        /// <summary>
        /// Creates a new location in the system.
        /// </summary>
        /// <param name="createLocation">The location data including name, address, city, province, postal code, and phone number</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created <see cref="Locations"/> object.
        /// Returns HTTP 200 (OK) with the created location on success.
        /// Returns HTTP 409 (Conflict) if a location with the same name already exists.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during creation.
        /// </returns>
        /// <response code="200">Returns the newly created location</response>
        /// <response code="409">If a location with the same name already exists</response>
        /// <response code="500">If an internal error occurs while creating the location</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/location
        ///     {
        ///         "name": "Downtown Branch",
        ///         "address_One": "123 Main Street",
        ///         "address_Two": "Suite 100",
        ///         "city": "Calgary",
        ///         "province": "Alberta",
        ///         "postal_Code": "T2P 1A1",
        ///         "phone_Number": "403-555-0100"
        ///     }
        ///
        /// Creates a new location with the provided information.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(Locations), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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


        /// <summary>
        /// Retrieves a specific location by its ID.
        /// </summary>
        /// <param name="location_id">The unique identifier of the location</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the <see cref="Location"/> object.
        /// Returns HTTP 200 (OK) with the location details on success.
        /// Returns HTTP 404 (Not Found) if the location doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the location with the specified ID</response>
        /// <response code="404">If the location is not found</response>
        /// <response code="500">If an internal error occurs while retrieving the location</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/location/123
        ///
        /// Returns the complete location details for the specified ID.
        /// </remarks>
        [HttpGet("{location_id}")]
        [ProducesResponseType(typeof(Locations), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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



        /// <summary>
        /// Updates an existing location's information.
        /// </summary>
        /// <param name="location_id">The unique identifier of the location to update</param>
        /// <param name="location_update">The updated location data</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated location object.
        /// Returns HTTP 200 (OK) with the updated location on success.
        /// Returns HTTP 404 (Not Found) if the location doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during the update.
        /// </returns>
        /// <response code="200">Returns the updated location</response>
        /// <response code="404">If the location is not found</response>
        /// <response code="500">If an internal error occurs while updating the location</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/location/123
        ///     {
        ///         "name": "Downtown Branch",
        ///         "address_One": "123 Main Street",
        ///         "address_Two": "Suite 100",
        ///         "city": "Calgary",
        ///         "province": "Alberta",
        ///         "postal_Code": "T2P 1A1",
        ///         "phone_Number": "403-555-0100"
        ///     }
        ///
        /// All fields in the request body are optional - only provided fields will be updated.
        /// </remarks>
        [HttpPut("{location_id}")]
        [ProducesResponseType(typeof(Locations), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Deletes a location from the system.
        /// </summary>
        /// <param name="location_id">The unique identifier of the location to delete</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns HTTP 200 (OK) with a success message when the location is deleted.
        /// Returns HTTP 404 (Not Found) if the location doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during deletion.
        /// </returns>
        /// <response code="200">Returns a success message when the location is deleted</response>
        /// <response code="404">If the location is not found</response>
        /// <response code="500">If an internal error occurs while deleting the location</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/location/123
        ///
        /// Permanently removes the location from the database.
        /// This may also remove associated menu-location relationships depending on cascade settings.
        /// </remarks>
        [HttpDelete("{location_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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



       /// <summary>
        /// Associates a menu with a specific location.
        /// </summary>
        /// <param name="location_id">The unique identifier of the location</param>
        /// <param name="menuLocationCreate">The menu location data containing the menu ID</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns HTTP 200 (OK) with a success message when the menu is added to the location.
        /// Returns HTTP 404 (Not Found) if the location or menu doesn't exist.
        /// Returns HTTP 409 (Conflict) if the menu is already assigned to this location.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during creation.
        /// </returns>
        /// <response code="200">Returns a success message when the menu is added to the location</response>
        /// <response code="404">If the location or menu is not found</response>
        /// <response code="409">If the menu is already assigned to this location</response>
        /// <response code="500">If an internal error occurs while adding the menu to the location</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/menulocation/123/menus
        ///     {
        ///         "menu_id": 456
        ///     }
        ///
        /// Creates an association between the specified menu and location.
        /// The menu and location must both exist before creating the association.
        /// </remarks>
        [HttpPost("{location_id}/menus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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



        /// <summary>
        /// Removes a menu from a specific location.
        /// </summary>
        /// <param name="location_id">The unique identifier of the location</param>
        /// <param name="menu_id">The unique identifier of the menu</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns HTTP 200 (OK) with a success message when the menu is removed from the location.
        /// Returns HTTP 404 (Not Found) if the menu-location association doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during removal.
        /// </returns>
        /// <response code="200">Returns a success message when the menu is removed from the location</response>
        /// <response code="404">If the menu-location association is not found</response>
        /// <response code="500">If an internal error occurs while removing the menu from the location</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/menulocation/123/menus/456
        ///
        /// Removes the association between the specified menu and location.
        /// The menu itself is not deleted, only its assignment to this location.
        /// </remarks>
        [HttpDelete("{location_id}/menus/{menu_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Retrieves all menu IDs associated with a specific location.
        /// </summary>
        /// <param name="location_id">The unique identifier of the location</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of menu IDs.
        /// Returns HTTP 200 (OK) with the list of menu IDs on success.
        /// Returns HTTP 404 (Not Found) if the location doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the list of menu IDs for the specified location</response>
        /// <response code="404">If the location is not found</response>
        /// <response code="500">If an internal error occurs while retrieving menu IDs</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/menulocation/123/menus
        ///
        /// Returns an array of menu IDs associated with the specified location.
        /// </remarks>
        [HttpGet("{location_id}/menus")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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