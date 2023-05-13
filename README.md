# STGenetics

This is a .NET 7 Web API built to fulfill the requirements of the developers test.

## Authentication
- All endpoints require JSON Web Token (JWT) authentication, to use it from swagger use the authorization button, then in the text field put the word bearer followed by the token generated in the endpoint log in.
Note: to create an user use the registration endpoint.

## Details
- The API is built in C# using the .NET 7 framework.
- Dapper is used as the Micro ORM.
- All endpoints are built using async/await for improved performance and scalability.

##Database construction
- To create the database, you need to execute the script named "01_Data_Base_Script.sql" located in the scripts folder. 
