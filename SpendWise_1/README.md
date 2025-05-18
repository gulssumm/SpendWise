# Programming Technology Lab
## Team

| Name Surname (initials) | GUID                                     |
| ----------------------- | ---------------------------------------- |
| Gulsum Cetinozlu        | {eba4d491-fbbb-435b-8d59-9a5fd8cc0b9e}   |
| Amarildo Fezollari      | {8a020147-2649-4520-8d43-a1c03c21e55b}   |

# SpendWise Financial Manager
> A financial transaction management system built using .NET Framework 4.7.2, following a three-layer architecture with MVVM design pattern for the presentation layer.

## Project OverView
> SpendWise allows users to manage their financial transactions through a user-friendly interface. Users can add, view, edit, and analyze both income and expenses with a UI pattern.

## Architecture
### Data Layer (SpendWise.Data)
- Responsible for data access and persistence
- Uses LINQ to SQL (ORM) for database operations
- Contains data models and repository implementations
- Provides abstracted API through interfaces

### Logic Layer (SpendWise.Logic)
- Contains business logic and data processing services
- Implements transaction-related operations
- Uses only the abstract Data layer API
- Provides services for the presentation layer

### Presentation Layer (SpendWise.Presentation)
- Implements the MVVM (Model-View-ViewModel) pattern
- Contains Master-Detail UI for transaction management
- Utilizes data binding for UI updates
- Uses commands for UI actions
- Dependency injection for testability

## Technology
- .NET Framework 4.7.2
- WPF (Windows Presentation Foundation)
- LINQ to SQL for data access
- SQL Server database
- MVVM architecture pattern
- MSTest for unit testing

## Project Structure
'''
SpendWise_1 Solution
├── SpendWise.Data
│   ├── Models
│   │   ├── CatalogItem.cs
│   │   ├── EventModel.cs
│   │   ├── FinancialTransactionModel.cs
│   │   ├── ProcessState.cs
│   │   ├── TransactionCategoryModel.cs
│   │   ├── UserEventModel.cs
│   │   └── UserModel.cs
│   ├── ITransactionRepository.cs
│   ├── MyDataModel.dbml (LINQ to SQL)
│   ├── TransactionProcessState.cs
│   └── TransactionRepository.cs
│
├── SpendWise.Logic
│   ├── Interfaces
│   │   └── ITransactionService.cs
│   └── Services
│       └── TransactionService.cs
│
├── SpendWise.Presentation
│   ├── Commands
│   │   └── RelayCommand.cs
│   ├── Converters
│   │   ├── ExpenseColorConverter.cs
│   │   └── InverseBoolConverter.cs
│   ├── Models
│   │   └── TransactionModel.cs
│   ├── ViewModels
│   │   ├── MainViewModel.cs
│   │   ├── TransactionDetailViewModel.cs
│   │   ├── TransactionListViewModel.cs
│   │   └── ViewModelBase.cs
│   ├── Views
│   │   └── MainWindow.xaml
│   └── Program.cs
│
└── SpendWise.Tests
    ├── ProcessStateTests.cs
    ├── TestDataGenerator.cs
    ├── TransactionRepositoryTests.cs
    ├── TransactionServiceTests.cs
    └── ViewModelTests
        ├── MainViewModelTests.cs
        ├── TransactionDetailViewModelTests.cs
        └── TransactionListViewModelTests.cs
'''
## Setup Instructions
1.Clone the repository
2.Open the solution in Visual Studio
3.Update the connection string in App.config for your SQL 4.Server instance
5.Build the solution
6.Run the database scripts (if provided) or use Entity 7.Framework migrations
8.Run the application

## Testing
> The project includes comprehensive unit tests for all layers:
- Data Layer Tests: Verify repository and data model functionality
- Logic Layer Tests: Ensure business logic works correctly
- Presentation Layer Tests: Validate ViewModel behavior
> Tests use both real implementations and mock objects to ensure proper isolation.

## Development Approach
> This project follows these development practices:
- MVVM Pattern: Clear separation between UI and business logic
- Dependency Injection: For loose coupling and testability
- Repository Pattern: For data access abstraction
- Interface-based Programming: Using abstractions rather than implementations
- Unit Testing: Complete test coverage for all layers