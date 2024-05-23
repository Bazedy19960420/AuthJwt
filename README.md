<h1><b>Authentication in Asp.Net8 Core</b></h1>
<hr/>

  
-first thing i am going to do is install	Microsoft.AspNetCore.Identity.EntityFrameworkCore.

-then we will create in modelfolder ( User)Class that inherits from identityUser class and i put my properties that i need and doesnt exist in identity user for now i just need (firstName,lastName).

-but i have to create my DbcontextClass to create my database
and that dbcontext class will inherit from identitydbcontext Class t because we want to integrate our context with Identity so we create one in the repoisotry Folder.

-then i will inject AddIdentity in services but i will change some configuration for the password to check! go and look at the extensions Method class in ConfigureIdentity Method it is easy and the code implements itself and doesnt need explaination.

-then i will Add the extension Method in services in program class,and i will add another extension method AddAuthentication,and in middleware i will add useAuthentication before App.useAuthorization.

-but it will not work ..why? because we didnt make the configuration for the database .. i will explain it we will use sqllite because it is good for our purpose and we dont want to distract from our subject Identity.just Add this in appsetting.json
 <b>"ConnectionStrings": {
  "DefaultConnection": "Data source = AuthDb.db"
}</b>

-and then install entityframework.sqllite library & entityframeworkcore & entityframeworkcore.tools and & entityframeworkcore.design.

-and add this line in program class :
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

-then in the packagemanageconsole:
Add-migration identityMigration   & update-database 

-then the entity framework will do the majic and create the user tables and roles and all of that stuff we will talk about it.

-but before we dive into implementation of Authentication and authorization we will seed roles in identityRoles intity...! checkAppDbcontextfile to see the way,and dont forget to add-migration and update-database.


<hr/>
<h2>Adding Controller for authentication:</h2>

-I create authenticationController class and i will put one httpmethod inside and it will be httppost to register the user;but before i implement this method we should create our userregisterdto .

-then we create the service that what will register the user in the db.

-we will use automapper for mapping the userregisterdto to the user.

-we will create the folder service and put IAuthentciationService Interface inside we put one method is RegisterUser; and we put inside also AuthenticationService to Implement this Method...! to see How check Service Folder.

-till now we didnt use Authentication we just register the user.

<hr/>
<h2>JWTAuthentication:</h2>

-json web tokent is a secure way to transmit data between two parties and it consists of three basic parts header,payload,signature; i will use it to implement the Authentication.
-first i install this library Microsoft.AspNetCore.Authentication.JwtBearer

-then i configure the jwtsetting in appsetting.json,then i Register the AddAuthentication middleware see(ConfigureJwt)method in ExtensionsMethods class then i add it in program class.
-then to protect endpoints we should write [Authorize] over it  and the the client should be authorized to get the response.

-then we should implement the method that authenticate the client , i will create userforAuthenticationDto and then i will create the service that authenticate this client...! see AutheticationService class.

-then we create postMethod to authenticate the user it took parameter userforAuthenticationDto and we use the validUser method we implemented in the service to see if the user valid or unAuthorized ; then if it Authorized we return token from the method createtoken.
<hr/>

<h2>RefreshToken:</h2>

-we use refreshToken for some secure reasons on of it we dont need if some one get the token so he can use it for a longtime
even if we changed the password.

-lets introduce the refresh token in our approach and see what happens:
	
1. First, the client authenticates with the authentication component by 
providing the credentials.
2. Then, the authentication component issues the access token and 
the refresh token.
3. After that, the client requests the resource endpoints for a protected 
resource by providing the access token.
4. The resource endpoint validates the access token and provides a 
protected resource.
5. Steps 3 & 4 keep on repeating until the access token expires.
6. Once the access token expires, the client requests a new access 
token by providing the refresh token.
7. The authentication component issues a new access token and 
refresh token.
8. Steps 3 through 7 keep on repeating until the refresh token expires.
9. Once the refresh token expires, the client needs to authenticate 
with the authentication server once again and the flow repeats from 
step 1.

-first we modify the user to have (refreshToken,expirytime)properties ,then migrate and updatedatabse.

-then we modify the createToken method in IAuthenticateService insteadof returning string i will return tokendto and it take one parameter bool .

-then i will create method generaterefreshtoken to generate the refreshtoken,then we will create getclaimPrincipal to get the claimPrincipal from expiredtoken.

-then we will modify the AuthenticateMethod in Authentication controller to return tokendto insteadof string.

<hr/>

<h2>references:</h2>

-https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding

-https://code-maze.com/asp-net-core-identity-series/

-https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0
