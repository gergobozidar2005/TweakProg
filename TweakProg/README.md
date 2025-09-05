# TweakProg - System Optimization Tool

A comprehensive Windows system optimization application with client-server architecture, featuring user management, license key system, and various cleanup tools.

## 🚀 Recent Improvements

### Security Enhancements
- ✅ Removed hardcoded credentials from configuration files
- ✅ Implemented environment-based configuration management
- ✅ Created separate development and production configuration files
- ✅ Added proper configuration validation

### API Consistency
- ✅ Standardized API endpoints across the application
- ✅ Fixed inconsistent port configurations
- ✅ Created centralized configuration management for client
- ✅ Added proper error handling for API calls

### Enhanced User Interface
- ✅ Modern, responsive admin dashboard with real-time statistics
- ✅ Progress indicators for long-running operations
- ✅ System information display panel
- ✅ Improved error handling and user feedback
- ✅ Status bar with operation status

### Expanded Functionality
- ✅ **Comprehensive Disk Cleanup**: Multi-step cleanup process including:
  - Windows temporary files
  - User temporary files
  - Browser cache (Chrome)
  - Old Windows log files
  - Recycle Bin emptying
- ✅ **Enhanced Temp Cleanup**: Progress tracking and detailed reporting
- ✅ **Improved Recycle Bin**: Better error handling and status reporting
- ✅ **System Information**: Real-time display of system stats

### Admin Dashboard
- ✅ Real-time statistics dashboard
- ✅ License management with API endpoints
- ✅ User management interface
- ✅ Recent activity monitoring
- ✅ Auto-refresh functionality

## 🏗️ Architecture

### Client Application (`TweakAppClient`)
- **Framework**: .NET 8.0 WPF
- **Features**: System optimization tools, hardware fingerprinting, license activation
- **Configuration**: Centralized configuration management via `app.config`

### Backend API (`TweakManagerBackend`)
- **Framework**: .NET 8.0 ASP.NET Core Web API
- **Database**: MySQL with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with email confirmation
- **Features**: User management, license system, admin dashboard

## 🛠️ Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- MySQL Server
- Visual Studio 2022 or VS Code

### Backend Setup
1. Update `appsettings.Development.json` with your database connection string
2. Configure SMTP settings for email functionality
3. Set up admin credentials in the development configuration
4. Run database migrations:
   ```bash
   cd TweakManagerBackend
   dotnet ef database update
   ```
5. Start the backend:
   ```bash
   dotnet run
   ```

### Client Setup
1. Update `app.config` with correct API endpoints
2. Build and run the client application
3. The application will guide you through registration and license activation

## 🔧 Configuration

### Backend Configuration
- **Database**: Configure connection string in `appsettings.json`
- **SMTP**: Set up email server settings for user notifications
- **Admin**: Configure default admin account in development settings

### Client Configuration
- **API Endpoints**: Configure in `app.config`
- **Logging**: Enable/disable logging via configuration

## 📊 Features

### System Optimization Tools
1. **Temporary File Cleanup**: Removes Windows and user temp files
2. **Recycle Bin Management**: Empties recycle bin safely
3. **Comprehensive Disk Cleanup**: Multi-step cleanup process
4. **Browser Cache Cleanup**: Removes Chrome cache files
5. **Log File Cleanup**: Removes old Windows log files

### User Management
- User registration with email confirmation
- Hardware-based license binding
- Admin role management
- User activity logging

### License System
- Hardware fingerprinting for license binding
- License key generation and management
- Activation tracking and validation
- Admin dashboard for license oversight

## 🔒 Security Features

- Hardware-based license binding
- Email confirmation for user registration
- Role-based access control
- Secure API endpoints
- Configuration-based credential management

## 📈 Admin Dashboard

The admin dashboard provides:
- Real-time system statistics
- License management interface
- User management tools
- Activity monitoring
- Recent logs display

## 🚧 Future Enhancements

- Registry cleanup functionality
- Startup program management
- System performance monitoring
- Automated cleanup scheduling
- Advanced reporting and analytics
- Multi-language support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions, please create an issue in the repository or contact the development team.

---

**Note**: This application is designed for Windows systems and requires appropriate permissions for system optimization operations.