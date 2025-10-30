using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
public class TestController : BaseController
{
    private readonly IUserService _userService;
    private readonly IVehicleModelService _vehicleModelService;
    private readonly IVehicleService _vehicleService;
    private readonly IRentalService _rentalService;

    private readonly string[] _firstNames = { "João", "Maria", "Pedro", "Ana", "Carlos", "Fernanda", "Lucas", "Julia", "Bruno", "Camila" };
    private readonly string[] _lastNames = { "Silva", "Santos", "Oliveira", "Souza", "Costa", "Ferreira", "Alves", "Pereira", "Lima", "Gomes" };
    private readonly string[] _domains = { "email.com", "teste.com", "demo.com", "example.com", "mail.com" };
    private readonly string[] _brands = { "Toyota", "Honda", "Ford", "Chevrolet", "Volkswagen", "Fiat", "Hyundai", "Nissan" };
    private readonly string[] _models = { "Corolla", "Civic", "Focus", "Cruze", "Golf", "Uno", "HB20", "March" };
    private readonly string[] _fuelTypes = { "Flex", "Gasolina", "Etanol", "Diesel" };
    private readonly string[] _engines = { "1.0", "1.4", "1.6", "1.8", "2.0" };
    private readonly string[] _cnhCategories = { "B", "AB", "AC", "C", "D" };

    public TestController(
        IUserService userService,
        IVehicleModelService vehicleModelService,
        IVehicleService vehicleService,
        IRentalService rentalService,
        ILogger<TestController> logger) : base(logger)
    {
        _userService = userService;
        _vehicleModelService = vehicleModelService;
        _vehicleService = vehicleService;
        _rentalService = rentalService;
    }

    [HttpPost("run-all-tests")]
    public async Task<IActionResult> RunAllTests()
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting comprehensive system test with random data");

            var testResults = new
            {
                Users = await TestUsersFlow(),
                VehicleModels = await TestVehicleModelsFlow(),
                Vehicles = await TestVehiclesFlow(),
                Rentals = await TestRentalsFlow(),
                Summary = "All tests completed successfully"
            };

            _logger.LogInformation("All tests completed successfully");
            return testResults;
        }, "RunAllTests");
    }

    [HttpPost("test-users")]
    public async Task<IActionResult> TestUsersFlow()
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting user flow test");

            var users = new List<UserResponseDto>();
            var random = new Random();

            for (int i = 0; i < 5; i++)
            {
                var firstName = _firstNames[random.Next(_firstNames.Length)];
                var lastName = _lastNames[random.Next(_lastNames.Length)];
                var email = $"{firstName.ToLower()}.{lastName.ToLower()}{random.Next(100, 999)}@{_domains[random.Next(_domains.Length)]}";

                var createDto = new CreateUserDto(firstName, lastName, email);
                var user = await _userService.CreateUserAsync(createDto);
                users.Add(user);
                var documentType = random.Next(2) == 0 ? "CPF" : "CNPJ";
                var documentValue = documentType == "CPF" 
                    ? GenerateRandomCPF() 
                    : GenerateRandomCNPJ();

                await _userService.AddDocumentToUserAsync(user.Id, new AddDocumentToUserDto(documentValue, documentType));

                var cnhNumber = GenerateRandomCNH();
                var expiryDate = DateTime.UtcNow.AddYears(random.Next(1, 5));
                var category = _cnhCategories[random.Next(_cnhCategories.Length)];

                await _userService.AddDriverLicenseAsync(user.Id, new AddDriverLicenseDto(cnhNumber, expiryDate, category));

                _logger.LogInformation("Created test user: {Email} with CNH category: {Category}", email, category);
            }

            var firstUser = users.First();
            await _userService.DeactivateUserAsync(firstUser.Id);
            await _userService.ActivateUserAsync(firstUser.Id);

            _logger.LogInformation("User flow test completed successfully. Created {Count} users", users.Count);

            return new
            {
                Message = "User flow test completed",
                UsersCreated = users.Count,
                Users = users,
                Operations = new[] { "Create", "AddDocument", "AddCNH", "Deactivate", "Activate" }
            };
        }, "TestUsersFlow");
    }

    [HttpPost("test-vehicle-models")]
    public async Task<IActionResult> TestVehicleModelsFlow()
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting vehicle models flow test");

            var vehicleModels = new List<VehicleModelResponseDto>();
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                var brand = _brands[random.Next(_brands.Length)];
                var model = _models[random.Next(_models.Length)];
                var year = random.Next(2018, 2025);
                var fuelType = _fuelTypes[random.Next(_fuelTypes.Length)];
                var engine = _engines[random.Next(_engines.Length)];
                var dailyRate = Math.Round((decimal)(random.NextDouble() * 200 + 50), 2);

                var createDto = new CreateVehicleModelDto(brand, model, year, fuelType, engine, dailyRate);
                var vehicleModel = await _vehicleModelService.CreateVehicleModelAsync(createDto);
                vehicleModels.Add(vehicleModel);

                _logger.LogInformation("Created vehicle model: {Brand} {Model} {Year} - R$ {DailyRate}/day", 
                    brand, model, year, dailyRate);
            }

            var firstModel = vehicleModels.First();
            await _vehicleModelService.DeactivateVehicleModelAsync(firstModel.Id);
            await _vehicleModelService.ActivateVehicleModelAsync(firstModel.Id);

            _logger.LogInformation("Vehicle models flow test completed successfully. Created {Count} models", vehicleModels.Count);

            return new
            {
                Message = "Vehicle models flow test completed",
                ModelsCreated = vehicleModels.Count,
                VehicleModels = vehicleModels,
                Operations = new[] { "Create", "Deactivate", "Activate" }
            };
        }, "TestVehicleModelsFlow");
    }

    [HttpPost("test-vehicles")]
    public async Task<IActionResult> TestVehiclesFlow()
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting vehicles flow test");

            var allModels = await _vehicleModelService.GetActiveVehicleModelsAsync();
            if (!allModels.Any())
            {
                await TestVehicleModelsFlow();
                allModels = await _vehicleModelService.GetActiveVehicleModelsAsync();
            }

            var vehicles = new List<VehicleResponseDto>();
            var random = new Random();

            for (int i = 0; i < 5; i++)
            {
                var licensePlate = GenerateRandomLicensePlate();
                var chassis = GenerateRandomChassis();
                var vehicleModel = allModels.ElementAt(random.Next(allModels.Count()));
                var vehicleType = (VehicleType)random.Next(1, 5);

                var createDto = new CreateVehicleDto(licensePlate, chassis, vehicleModel.Id, vehicleType);
                var vehicle = await _vehicleService.CreateVehicleAsync(createDto);
                vehicles.Add(vehicle);

                var mileage = random.Next(0, 100000);
                await _vehicleService.UpdateVehicleMileageAsync(vehicle.Id, new UpdateVehicleDto(mileage));

                _logger.LogInformation("Created vehicle: {LicensePlate} - {VehicleType} with {Mileage} km", 
                    licensePlate, vehicleType, mileage);
            }

            var firstVehicle = vehicles.First();
            await _vehicleService.SendVehicleToMaintenanceAsync(firstVehicle.Id);
            await _vehicleService.MakeVehicleAvailableAsync(firstVehicle.Id);
            await _vehicleService.RestrictVehicleAsync(firstVehicle.Id, "Test restriction");
            await _vehicleService.MakeVehicleAvailableAsync(firstVehicle.Id);

            _logger.LogInformation("Vehicles flow test completed successfully. Created {Count} vehicles", vehicles.Count);

            return new
            {
                Message = "Vehicles flow test completed",
                VehiclesCreated = vehicles.Count,
                Vehicles = vehicles,
                Operations = new[] { "Create", "UpdateMileage", "Maintenance", "MakeAvailable", "Restrict" }
            };
        }, "TestVehiclesFlow");
    }

    [HttpPost("test-rentals")]
    public async Task<IActionResult> TestRentalsFlow()
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting rentals flow test");

            var allUsers = await _userService.GetAllUsersAsync();
            var eligibleUsers = allUsers.Where(u => u.IsEligibleToRent).ToList();
            
            if (!eligibleUsers.Any())
            {
                await TestUsersFlow();
                allUsers = await _userService.GetAllUsersAsync();
                eligibleUsers = allUsers.Where(u => u.IsEligibleToRent).ToList();
            }

            var availableVehicles = await _vehicleService.GetAvailableVehiclesAsync();
            if (!availableVehicles.Any())
            {
                await TestVehiclesFlow();
                availableVehicles = await _vehicleService.GetAvailableVehiclesAsync();
            }

            var rentals = new List<RentalResponseDto>();
            var random = new Random();

            for (int i = 0; i < Math.Min(3, Math.Min(eligibleUsers.Count, availableVehicles.Count())); i++)
            {
                var user = eligibleUsers[i];
                var vehicle = availableVehicles.ElementAt(i);
                var startDate = DateTime.UtcNow.AddDays(random.Next(0, 7));
                var endDate = startDate.AddDays(random.Next(1, 15));

                var createDto = new CreateRentalDto(user.Id, vehicle.Id, startDate, endDate);
                var rental = await _rentalService.CreateRentalAsync(createDto);
                rentals.Add(rental);

                await _rentalService.ConfirmRentalAsync(rental.Id);

                _logger.LogInformation("Created rental: {RentalId} for user {UserEmail} with vehicle {LicensePlate}", 
                    rental.Id, user.Email, vehicle.LicensePlate);
            }

            if (rentals.Any())
            {
                var firstRental = rentals.First();
                
                var newEndDate = firstRental.EndDate.AddDays(5);
                await _rentalService.ExtendRentalAsync(firstRental.Id, new ExtendRentalDto(newEndDate));

                var finalMileage = firstRental.Vehicle?.Mileage + random.Next(100, 1000) ?? 1000;
                await _rentalService.CompleteRentalAsync(firstRental.Id, new CompleteRentalDto(finalMileage, "Test completion"));
            }

            _logger.LogInformation("Rentals flow test completed successfully. Created {Count} rentals", rentals.Count);

            return new
            {
                Message = "Rentals flow test completed",
                RentalsCreated = rentals.Count,
                Rentals = rentals,
                Operations = new[] { "Create", "Confirm", "Extend", "Complete" }
            };
        }, "TestRentalsFlow");
    }

    [HttpGet("system-status")]
    public async Task<IActionResult> GetSystemStatus()
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting system status");

            var allUsers = await _userService.GetAllUsersAsync();
            var activeUsers = await _userService.GetActiveUsersAsync();
            var eligibleUsers = allUsers.Where(u => u.IsEligibleToRent);

            var allVehicleModels = await _vehicleModelService.GetAllVehicleModelsAsync();
            var activeVehicleModels = await _vehicleModelService.GetActiveVehicleModelsAsync();

            var allVehicles = await _vehicleService.GetAllVehiclesAsync();
            var availableVehicles = await _vehicleService.GetAvailableVehiclesAsync();

            var allRentals = await _rentalService.GetAllRentalsAsync();
            var activeRentals = await _rentalService.GetActiveRentalsAsync();
            var overdueRentals = await _rentalService.GetOverdueRentalsAsync();

            var status = new
            {
                Users = new
                {
                    Total = allUsers.Count(),
                    Active = activeUsers.Count(),
                    EligibleToRent = eligibleUsers.Count()
                },
                VehicleModels = new
                {
                    Total = allVehicleModels.Count(),
                    Active = activeVehicleModels.Count()
                },
                Vehicles = new
                {
                    Total = allVehicles.Count(),
                    Available = availableVehicles.Count(),
                    Rented = allVehicles.Count(v => v.Status == VehicleStatus.Rented),
                    Maintenance = allVehicles.Count(v => v.Status == VehicleStatus.Maintenance),
                    Restricted = allVehicles.Count(v => v.Status == VehicleStatus.Restricted)
                },
                Rentals = new
                {
                    Total = allRentals.Count(),
                    Active = activeRentals.Count(),
                    Overdue = overdueRentals.Count(),
                    Completed = allRentals.Count(r => r.Status == RentalStatus.Completed),
                    Cancelled = allRentals.Count(r => r.Status == RentalStatus.Cancelled)
                },
                SystemHealth = "Operational",
                LastCheck = DateTime.UtcNow
            };

            _logger.LogInformation("System status retrieved successfully");
            return status;
        }, "GetSystemStatus");
    }

    private string GenerateRandomCPF()
    {
        var random = new Random();
        return $"{random.Next(100, 999)}.{random.Next(100, 999)}.{random.Next(100, 999)}-{random.Next(10, 99)}";
    }

    private string GenerateRandomCNPJ()
    {
        var random = new Random();
        return $"{random.Next(10, 99)}.{random.Next(100, 999)}.{random.Next(100, 999)}/{random.Next(1000, 9999)}-{random.Next(10, 99)}";
    }

    private string GenerateRandomCNH()
    {
        var random = new Random();
        return random.Next(10000000, 99999999).ToString() + random.Next(100, 999);
    }

    private string GenerateRandomLicensePlate()
    {
        var random = new Random();
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var plate = "";
        
        if (random.Next(2) == 0)
        {
            for (int i = 0; i < 3; i++)
                plate += letters[random.Next(letters.Length)];
            plate += random.Next(1000, 9999);
        }
        else
        {
            for (int i = 0; i < 3; i++)
                plate += letters[random.Next(letters.Length)];
            plate += random.Next(0, 10);
            plate += letters[random.Next(letters.Length)];
            plate += random.Next(10, 99);
        }
        
        return plate;
    }

    private string GenerateRandomChassis()
    {
        var random = new Random();
        var chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        var chassis = "";
        
        for (int i = 0; i < 17; i++)
            chassis += chars[random.Next(chars.Length)];
            
        return chassis;
    }
}