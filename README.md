🛠️ AutoParts & Maintenance Management System (POS)

A robust, enterprise-grade Point of Sale and Maintenance Tracking System specifically designed for automotive service centers and spare parts retailers. Built with ASP.NET Core 6 MVC, this system ensures precision in inventory management and excellence in customer service.

🔄 System Workflow & Logic (The Process)
This diagram illustrates how the system handles a typical maintenance job and its financial impact:
graph TD
    A[Customer Arrival] --> B{Vehicle Registered?}
    B -- No --> C[Register Car & Owner]
    B -- Yes --> D[Create Maintenance Ticket]
    D --> E[Technician Assessment]
    E --> F[Assign Spare Parts from Inventory]
    F --> G{Stock Check}
    G -- Available --> H[Deduct Stock & Apply to Ticket]
    G -- Out of Stock --> I[Generate Purchase Order]
    H --> J[Complete Repair]
    J --> K[Generate Final Invoice]
    K --> L[Post Financial Record to Accounts]
    L --> M[Close Ticket]


    🧠 Logic Architecture:
Inventory-Maintenance Linking: When a part is added to a job, the system automatically checks the StockQuantity. It prevents closing a ticket if parts are not available.

Financial Integrity: Invoices are calculated dynamically: Total = (Sum of Parts Price) + (Labor/Workmanship Fees) - (Discounts).

Data Persistence: Uses Entity Framework Core with Transactional logic to ensure that if a payment fails, the stock is not deducted.

🌟 Key Features & Modules
🚘 Maintenance & Vehicle Management
Vehicle Lifecycle Tracking: Monitor service history by VIN or License Plate.

Maintenance Logs: Detailed records of repairs and mechanic assignments.

Status Tracking: Real-time updates from "Pending" to "Completed."

📦 Inventory & Warehouse Control
Real-time Stock Tracking: Automatic deduction of parts upon sale.

Low Stock Alerts: Notifications for critical parts reordering.

Categorization: Organize parts by brand and model compatibility.

💰 Accounting & POS
Dynamic Invoicing: Professional PDF-ready invoices.

Session Management: Secure user sessions for cashiers and admins.

Profit Reporting: Daily/Monthly financial summaries.

🛠️ Installation & Setup Guide

1. Clone the Repository
git clone https://github.com/yourusername/AutoPartsPOS.git
cd AutoPartsPOS

2. Install Dependencies (NuGet Packages)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

3. Database Initialization
Open SSMS and connect to .\SQLEXPRESS.

Right-click Databases > Restore Database.

Select the provided .bak file.

Verify the connection string in appsettings.json.

📦 Deployment (Publishing)
To create a standalone executable for the client:
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

📂 Project Structure
/Controllers: Business logic (Inventory, Maintenance, Accounts).

/Data: Database context and EF Core configurations.

/Models: Database entities.

/Views: Razor-based responsive UI.

/wwwroot: Static assets (CSS, JS, Images).

🛡️ License & Trial Terms
Trial Period: 3 Days (Managed via %AppData%/sys_config.data).

Commercial License: Contact the developer for full activation.


🤝Contact & Support
Developed with ❤️ by Mohamed Khaled.


WhatsApp: [01092320944]

Email: [Hamok550@gmail.com]
   
