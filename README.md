### A way for me to test things out in every regard

# Project Structure
/YourSolution.sln
/src
    /Api                      # ASP.NET Core WebAPI project
    /Orleans.SiloHost         # Orleans Silo host project
    /Orleans.Grains           # Orleans Grain Implementations
    /Orleans.GrainInterfaces  # Orleans Grain Interfaces
    /Shared                   # DTOs, contracts, common utilities
    /Domain                   # Business logic and entities (optional, see below)
    /Persistence              # EF Core or other storage implementations (optional)
    /Infrastructure           # External integrations (e.g., messaging, file storage)
    /Application              # CQRS, MediatR, use cases (optional, for complex apps)
