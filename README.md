# SAML 2.0 authentication using Sustainsys .NET Library
Implement secure SAML 2.0 authentication using Sustainsys .NET Library for .NET Core App and Legacy .NET Framework with Azure AD IDP.

# Introduction
Security Assertion Markup Language (SAML) is a widely adopted standard for secure Single Sign-On (SSO) authentication. It allows users to access multiple applications and services with a single set of login credentials. SAML eliminates the need for separate authentication processes for each application, enhancing user convenience and security. In this article, we will explore how to implement SAML authentication using the Sustainsys .NET library, a powerful toolset that simplifies the integration of SAML into .NET applications.

## Introducing Sustainsys .NET Library
Sustainsys .NET library is a robust and flexible open-source library that simplifies SAML 2.0 authentication implementation in .NET applications. It provides an extensive set of features, including support for both IdP and SP roles, advanced configuration options, and seamless integration with ASP.NET applications. It was previously called Kentor Auth Services. The library provides support for
- ASP.NET web forms
- ASP.NET MVC
- OWIN middleware
- ASP.NET Core 2
- Identityserver integration

## Setting up Sustainsys for SAML Authentication
To get started with SAML authentication using Sustainsys, follow these steps considering Azure AD as IdP:
1. Configuring SAML based single sign-on for non-gallery applications

a) Login to Azure portal. Enterprise applications and create your own new application. Select "Integrate any other application you don't find in the gallery (Non-gallery)

![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/693851de-9cb2-441d-b70c-4be8c64ee450)

b) Open the app → Single sign-on and select SAML
![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/05fb7019-062a-4c3a-8c1b-56a55374c31b)

c) Configure the "Basic SAML configuration" with the redirect URL's as per the application
![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/ce214a58-5e8b-4fd0-b479-926b9c1a332f)

d) Once above step is complete, below mentioned configuration values will be required in the app

**Basic SAML Configuration**: Identifier (Entity ID)
**SAML Certificates**: App Federation Metadata Url | Certificate (Base64) - Download it
**Set up {app name}**: Login URL | Azure AD Identifier | Logout URL

e) Users and groups - Add the user/group to assign the users or groups to User role for the application. If this is not done, you might get error code "AADSTS50105"

2. Download the below Repository and update the configurations in respective project configuration files (appsettings.json | web.config) from the above registered app. Upload the base 64 certificate to the project directory.

## SAML 2.0 authentication in .NET 6 App and .NET API
![Apparch](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/5d6be03f-a3fe-4c5d-9b24-1300802d3b29)

There are two .NET 6 projects in the solution
1. AspNetCore is the front-end App which implements SAML authentication.
2. AspNetCore.API project is API secured and validates the JWT token passed by the front-end app.

Update the configurations mentioned in appsettings.json of both project and copy the base 64 certificate to root directory of AspNetCore project.

In the AspNetCore Project, CreateJwtSecurityToken method: Creates JWT Token using Issuer, audience, claims and credentials and pass the token to the API request.
API validates the JWT token using the above validation parameters: issuer, key and expiration.

Home Page:

![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/758b6941-6f6b-40c9-a895-4f8486320867)

After successful login:

![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/3691b1e8-79e0-46c0-9b2d-9e51f6872b8b)

## SAML 2.0 authentication in ASP.NET MVC with .NET Framework
The SampleHttpModuleApplication project is ASP.NET MVC app using .NET framework 4.7.2. The use case considered is to transform your legacy app, where you want to replace form based authentication with SSO authentication using SAML 2.0. The configurations need to be changed in the web.config and the base64 certificate need to be copied to ~/App_Data directory

Home Page:

![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/8bf58c52-d5ef-44e3-9f54-e25526417d3f)

Once SAML authentication is successful

![image](https://github.com/prabhasinamdar/dotnet-saml-sustainsys/assets/45000018/52463ff4-4abb-4233-b064-e205fac650af)

## Benefits of Using Sustainsys for SAML Authentication
1. Simplified Integration: Sustainsys abstracts the complexity of SAML implementation, making it easier to integrate SAML into .NET applications.
2. Robust Security: SAML authentication enhances security by relying on digital signatures and encryption to protect user data during transmission.
3. Support for Multiple Identity Providers: Sustainsys supports integrating with multiple IdPs, enabling you to work with various identity providers seamlessly.
4. Active Development and Community Support: As an open-source library, Sustainsys benefits from active development and a supportive community, ensuring timely updates and bug fixes.

