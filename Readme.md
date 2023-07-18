# Login API
This API provides endpoints for user registration, login, and user information retrieval. It also provides endpoints for user verification, password recovery, and password change.

<div style="text-align: right; margin: 10px 0px 15px 50px; ">
  <a href="./Readme.md" style="margin-right: 20px; background-color: #4CAF50; color: white; padding: 5px 20px; text-decoration: none; border-radius: 4px; ">Language: EN</a>
  <a href="./Readme.es.md" style="background-color: #4CAF50; color: white; padding: 5px 20px; text-decoration: none; border-radius: 4px;" >Language: ES</a>
</div>

## Table of Contents
- [Requirements](#requirements)
- [Configuration](#configuration)
  - [Database](#database)
  - [JWT](#jwt)
  - [Email](#email)
- [Execution](#execution)
- [Available Routes](#available-routes)
- [API Usage](#api-usage)
  - [Registrer Usuario](#registrer-usuario)
  - [User Verification](#user-verification)
  - [User Verification Resend](#user-verification-resend)
  - [User Login](#user-login)
  - [User Information](#user-information)
  - [User Password Recovery](#user-password-recovery)
  - [User Password Change](#user-password-change)
- [usage examples](#usage-examples)
  - [Registrer Usuario](#registrer-usuario-1)
  - [User Verification](#user-verification-1)
  - [User Verification Resend](#user-verification-resend-1)
  - [User Login](#user-login-1)
  - [User Information](#user-information-1)
  - [User Password Recovery](#user-password-recovery-1)
  - [User Password Change](#user-password-change-1)
- [License](#license) 


## Requirements
   - [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
   - [vscode](https://code.visualstudio.com/download)
   - [vscode C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
   - [vscode NuGet Package Manager](https://marketplace.visualstudio.com/items?itemName=jmrog.vscode-nuget-package-manager)
   - [Docker](https://www.docker.com/products/docker-desktop)
   - [Docker Compose](https://docs.docker.com/compose/install/)
   - [Postman](https://www.postman.com/downloads/)
   - [MariaDB](https://mariadb.org/download/)

## Configuration
  1. Create a file named appsettings.json.
  2. Insert the environment variables into the appsettings.json file following the example in the appsettings.example.json file.
  3. Run the command `dotnet restore` to install the dependencies.
  4. Run the command `dotnet ef database update` to create the tables in the database.

  ### Database 
  1. The database connection data can be found in the docker-compose.yml file.
  2. Paste the connection data into the appsettings.json file following the example in the appsettings.example.json file.

  ### JWT
   1. Create a secret key for the JWT token (the token text should be base64).
   2. Generate a token key using the command: 
``` bash
openssl rand -base64 32
```
### Email
  1. Create an account on [ethereal](https://ethereal.email/create).
  2. Paste the account data into the appsettings.json file following the example in the appsettings.example.json file.


## Execution
Is necesary have realice the configuration steps before
  1. Ejecuta el comando `docker-compose up -d` para levantar el contenedor de la base de datos.
  2. Ejecuta el comando `dotnet run` para levantar el servidor.
  3. Ejecuta el comando `dotnet watch run` para levantar el servidor en modo desarrollo.

## Available Routes

- `POST /register`: Registers a new user in the system and sends a verification email.
- `GET /verify`: Verifies the user.
- `GET /resend`: Resends the verification email.
- `POST /login`: Logs in the user and returns an access token.
- `GET /user`: Gets information of the authenticated user.
- `GET /recover`: Recovers the user's password and sends an email for password change.
- `GET /user`: Changes the user's password and verifies the old password.

## API Usage

### Registrer Usuario

- **URL**: `/register`
- **Método**: `POST`
- **Request Parameters**:

  | Body        | Type   | Description       |
  | ----------- | ------ | ----------------- |
  | `username`  | String | user name |
  | `user_handle`| String | name id user  |
  | `password`  | String | password        |
  | `email`     | String | Email user|

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body: A verification email has been sent to your email address. Please verify your account to log in.
Error Response:

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body: Error message


### User Verification
- **URL**: `/verify`
- **Método**: `GET`
- **Request Parameters**:

    | Params        | Type   | Description       |
    | -----------  | ------ | ----------------- |
    | `code`       | String | verificaicon Token |

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body:  Account activated successfully.

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body: Error message

### User Verification Resend
- **URL**: `/resend`
- **Método**: `GET`
- **Request Parameters**:

    | Params        | Type   | Description       |
    | -----------  | ------ | ----------------- |
    | `email`      | String | verification Token |

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body:  A verification email has been sent to your email address. Please verify your account to log in.

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body: User does not exist.

### User Login

- **URL**: `/login`
- **Método**: `POST`
- **Request Parameters**:

  | Params        | Type   | Description       | 
  | ----------- | ------ | ----------------- |
  | `name`      | String | Email y/o nameHandle |
  | `password`  | String | Contraseña        |

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body:  Access token for future authentication.

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body: Error message

### Get User Information

- **URL**: `/user`
- **Método**: `GET`
- **Request Headers**:
  - `Authorization: Bearer {token}`

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body:  Information of the authenticated user.

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body: The token has expired

### Password Recovery Email

- **URL**: `/recover`
- **Método**: `POST`
- **Request Parameters**:

  | Params        | Type   | Description       | 
  | -----------   | ------ | ----------------- |
  | `email`       | String | User Email |

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body:  A password recovery email has been sent to your email address.

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body:  Insert a valid user

### Token and Password Change

- **URL**: `/resent`
- **Método**: `POST`
- **Request Headers**:
  - `Authorization: Bearer {token}`

    | body              | Type   | Description       |
    | -----------       | ------ | ----------------- |
    | `newpassword`     | String | Email del usuario |
    | `password`        | String | Email del usuario |

- **Successful Response**:
  - Status code: 201 (Created)
    - Response Body: Information of the authenticated user.

- **Error Response**:
    - Status code: 401 (Unauthorized)
        - Response Body:  Insert a valid user

## Usage Examples

### Curl 
``` bash
# User Registration
curl -X POST -H "Content-Type: application/json" -d '{"username":"example_user","password":"secure_password","email":"user@example.com"}' http://your-api.com/register

# User Verification
curl -X GET -H "Content-Type: application/json" -d '{"code":"verification_token"}' http://your-api.com/verify

# Resend Verification Email
curl -X GET -H "Content-Type: application/json" -d '{"email":"user@example.com"}' http://your-api.com/resend

# User Login
curl -X POST -H "Content-Type: application/json" -d '{"username":"example_user","password":"secure_password"}' http://your-api.com/login

# Get User Information
curl -X GET -H "Authorization: Bearer {access_token}" http://your-api.com/user

# Password Recovery Email
curl -X POST -H "Content-Type: application/json" -d '{"email":"user@example.com"}' http://your-api.com/recover

# Token and Password Change
curl -X POST -H "Authorization: Bearer {access_token}" -H "Content-Type: application/json" -d '{"newpassword":"new_password","password":"current_password"}' http://your-api.com/reset
```

### Javascript Fetch

```javascript
// User Registration
fetch('http://your-api.com/register', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: 'example_user',
    password: 'secure_password',
    email: 'user@example.com'
  })
});

// User Verification
fetch('http://your-api.com/verify?code=verification_token', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Resend Verification Email
fetch('http://your-api.com/resend?email=user@example.com', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json'
  }
});

// User Login
fetch('http://your-api.com/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: 'example_user',
    password: 'secure_password'
  })
});

// Get User Information
fetch('http://your-api.com/user', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer {access_token}'
  }
});

// Password Recovery Email
fetch('http://your-api.com/recover', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    email: 'user@example.com'
  })
});

// Token and Password Change
fetch('http://your-api.com/reset', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer {access_token}',
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    newpassword: 'new_password',
    password: 'current_password'
  })
});
```

### Python Requests

```python
import requests

# User Registration
register_url = 'http://your-api.com/register'
register_data = {
    'username': 'example_user',
    'password': 'secure_password',
    'email': 'user@example.com'
}
response = requests.post(register_url, json=register_data)
print(response.json())

# User Verification
verification_url = 'http://your-api.com/verify'
verification_data = {
    'code': 'verification_token'
}
response = requests.get(verification_url, params=verification_data)
print(response.json())

# Resend Verification Email
resend_url = 'http://your-api.com/resend'
resend_data = {
    'email': 'user@example.com'
}
response = requests.get(resend_url, params=resend_data)
print(response.json())

# User Login
login_url = 'http://your-api.com/login'
login_data = {
    'username': 'example_user',
    'password': 'secure_password'
}
response = requests.post(login_url, json=login_data)
print(response.json())

# Get User Information
user_url = 'http://your-api.com/user'
headers = {
    'Authorization': 'Bearer {access_token}'
}
response = requests.get(user_url, headers=headers)
print(response.json())

# Password Recovery Email
recover_url = 'http://your-api.com/recover'
recover_data = {
    'email': 'user@example.com'
}
response = requests.post(recover_url, json=recover_data)
print(response.json())

# Token and Password Change
reset_url = 'http://your-api.com/reset'
reset_data = {
    'newpassword': 'new_password',
    'password': 'current_password'
}
headers = {
    'Authorization': 'Bearer {access_token}'
}
response = requests.post(reset_url, headers=headers, json=reset_data)
print(response.json())

```

### C#

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class Program
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main()
    {
        // User Registration
        string registerUrl = "http://your-api.com/register";
        var registerData = new
        {
            username = "example_user",
            password = "secure_password",
            email = "user@example.com"
        };
        var registerResponse = await PostAsync(registerUrl, registerData);
        Console.WriteLine(await registerResponse.Content.ReadAsStringAsync());

        // User Verification
        string verificationUrl = "http://your-api.com/verify";
        var verificationData = new
        {
            code = "verification_token"
        };
        var verificationResponse = await GetAsync(verificationUrl, verificationData);
        Console.WriteLine(await verificationResponse.Content.ReadAsStringAsync());

        // Resend Verification Email
        string resendUrl = "http://your-api.com/resend";
        var resendData = new
        {
            email = "user@example.com"
        };
        var resendResponse = await GetAsync(resendUrl, resendData);
        Console.WriteLine(await resendResponse.Content.ReadAsStringAsync());

        // User Login
        string loginUrl = "http://your-api.com/login";
        var loginData = new
        {
            username = "example_user",
            password = "secure_password"
        };
        var loginResponse = await PostAsync(loginUrl, loginData);
        Console.WriteLine(await loginResponse.Content.ReadAsStringAsync());

        // Get User Information
        string userUrl = "http://your-api.com/user";
        string accessToken = "{access_token}";
        var userResponse = await GetAsync(userUrl, accessToken);
        Console.WriteLine(await userResponse.Content.ReadAsStringAsync());

        // Password Recovery Email
        string recoverUrl = "http://your-api.com/recover";
        var recoverData = new
        {
            email = "user@example.com"
        };
        var recoverResponse = await PostAsync(recoverUrl, recoverData);
        Console.WriteLine(await recoverResponse.Content.ReadAsStringAsync());

        // Token and Password Change
        string resetUrl = "http://your-api.com/reset";
        var resetData = new
        {
            newpassword = "new_password",
            password = "current_password"
        };
        var resetResponse = await PostAsync(resetUrl, resetData, accessToken);
        Console.WriteLine(await resetResponse.Content.ReadAsStringAsync());
    }

    private static async Task<HttpResponseMessage> PostAsync(string url, object data, string accessToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(data),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        return await client.SendAsync(request);
    }

    private static async Task<HttpResponseMessage> GetAsync(string url, object data, string accessToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (data != null)
        {
            var queryString = string.Join("&", Newtonsoft.Json.Linq.JObject.FromObject(data)
                .Properties()
                .Select(p => p.Name + "=" + Uri.EscapeDataString(p.Value.ToString())));
            request.RequestUri = new Uri(url + "?" + queryString);
        }
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        return await client.SendAsync(request);
    }
}
```
Please note that in the examples provided, you need to replace the placeholder URLs with the actual URLs of your API and replace {access_token} with the actual access token received from the login request.

## licence 
this project is under the MIT license
