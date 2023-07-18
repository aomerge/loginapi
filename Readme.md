# API de Inicio de Sesión
Esta API proporciona endpoints para el registro de usuarios, inicio de sesión y obtener información del usuario.

<div style="text-align: right; margin: 10px 0px 15px 50px; ">
  <a href="./Readme.md" style="margin-right: 20px; background-color: #4CAF50; color: white; padding: 5px 20px; text-decoration: none; border-radius: 4px; ">Lenguage: EN</a>
  <a href="./Readme.es.md" style="background-color: #4CAF50; color: white; padding: 5px 20px; text-decoration: none; border-radius: 4px;" >Lenguage: ES</a>
</div>

## Requisitos
   - [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
   - [vscode](https://code.visualstudio.com/download)
   - [vscode C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
   - [vscode NuGet Package Manager](https://marketplace.visualstudio.com/items?itemName=jmrog.vscode-nuget-package-manager)
   - [Docker](https://www.docker.com/products/docker-desktop)
   - [Docker Compose](https://docs.docker.com/compose/install/)
   - [Postman](https://www.postman.com/downloads/)
   - [MariaDB](https://mariadb.org/download/)

## Configuración
  1. Crea un archivo llamado appsettings.json.  
  4. Inserta las variables de entorno en el archivo appsettings.json siguiendo el ejemplo del archivo appsettings.example.json.
  5. Ejecuta el comando `dotnet restore` para instalar las dependencias.
  6. Ejecuta el comando `dotnet ef database update` para crear las tablas en la base de datos.

  ### Database 
  1. Los datos de la coneccion a la base de datos se encuentran en el archivo docker-compose.yml.
  2. Pega los datos de la coneccion en el archivo appsettings.json siguiendo el ejemplo del archivo appsettings.example.json.

  ### jwt
   1. Crea una clave secreta para el token jwt (El texto del token tiene que ser base 64).
   2. Genera una clave del token con el comando: 
``` bash
openssl rand -base64 32
```
### Email
  1. Crea una cuenta en [ethereal](https://ethereal.email/create).
  2. Pega los datos de la cuenta en el archivo appsettings.json siguiendo el ejemplo del archivo appsettings.example.json.


## Ejecución
  1. Ejecuta el comando `docker-compose up -d` para levantar el contenedor de la base de datos.
  2. Ejecuta el comando `dotnet run` para levantar el servidor.
  3. Ejecuta el comando `dotnet watch run` para levantar el servidor en modo desarrollo.

## Rutas Disponibles

- `POST /register`: Registra un nuevo usuario en el sistema y envia un coreo de verificacion.
- `GET /verify`: Verifica el usuario.
- `GET /resend`: Reenvia el usuario de verificacion.
- `POST /login`: Inicia sesión y devuelve un token de acceso.
- `GET /user`: Obtiene información del usuario autenticado.
- `GET /recover`: Recupera la contraseña del usuario y envia un email para cambio de contraseña.
- `GET /user`: Cambia la contraseña del usuario y comprueva la antigua contraseña.

## Uso de la api

### Registro de Usuario

- **URL**: `/register`
- **Método**: `POST`
- **Parámetros de la solicitud**:

  | Body        | Tipo   | Descripción       |
  | ----------- | ------ | ----------------- |
  | `username`  | String | Nombre de usuario |
  | `user_handle`| String | Nombre de usuario |
  | `password`  | String | Contraseña        |
  | `email`     | String | Correo electrónico|

- **Respuesta exitosa**:
  - Código de estado: 201 (Creado)
  - Cuerpo de la respuesta: Enviamos un correo electrónico de verificación a su dirección de correo electrónico. Por favor, verifique su cuenta para iniciar sesión.

- **Respuesta de error**:
  - Código de estado: 401 (Solicitud incorrecta)
  - Cuerpo de la respuesta: Mensaje de error

### Verificacion de usuario
- **URL**: `/verify`
- **Método**: `GET`
- **Parámetros de la solicitud**:

    | Params       | Tipo   | Descripción       |
    | -----------  | ------ | ----------------- |
    | `code`       | String | Token de verificaicon |

- **Respuesta exitosa**:
  - Código de estado: 201 (Creado)
  - Cuerpo de la respuesta: Cuenta activada exitosamente.

- **Respuesta de error**:
  - Código de estado: 401 (Solicitud incorrecta)
  - Cuerpo de la respuesta: Mensaje de error.

### Verificacion de usuario
- **URL**: `/resend`
- **Método**: `GET`
- **Parámetros de la solicitud**:

    | Params       | Tipo   | Descripción       |
    | -----------  | ------ | ----------------- |
    | `email`      | String | Token de verificaicon |

- **Respuesta exitosa**:
  - Código de estado: 201 (Creado)
  - Cuerpo de la respuesta: Enviamos un correo electrónico de verificación a su dirección de correo electrónico. Por favor, verifique su cuenta para iniciar sesión..

- **Respuesta de error**:
  - Código de estado: 401 (Solicitud incorrecta)
  - Cuerpo de la respuesta: El usuario no existe.

### Inicio de Sesión

- **URL**: `/login`
- **Método**: `POST`
- **Parámetros de la solicitud**:

  | Parámetro   | Tipo   | Descripción       |
  | ----------- | ------ | ----------------- |
  | `name`      | String | Email y/o nameHandle |
  | `password`  | String | Contraseña        |

- **Respuesta exitosa**:
  - Código de estado: 200 (OK)
  - Cuerpo de la respuesta: Token de acceso para autenticación futura

- **Respuesta de error**:
  - Código de estado: 401 (No autorizado)
  - Cuerpo de la respuesta: Mensaje de error

### Obtener Información del Usuario

- **URL**: `/user`
- **Método**: `GET`
- **Encabezados de la solicitud**:
  - `Authorization: Bearer {token}`

- **Respuesta exitosa**:
  - Código de estado: 200 (OK)
  - Cuerpo de la respuesta: Información del usuario autenticado

- **Respuesta de error**:
  - Código de estado: 401 (No autorizado)
  - Cuerpo de la respuesta: El token ha expirado

### Email de recuperacion de contraseña

- **URL**: `/recover`
- **Método**: `POST`
- **Encabezados de la solicitud**:

  | params        | Tipo   | Descripción       |
  | -----------   | ------ | ----------------- |
  | `email`       | String | Email del usuario |

- **Respuesta exitosa**:
  - Código de estado: 200 (OK)
  - Cuerpo de la respuesta: Enviamos un correo electrónico de recuperación de contraseña a su dirección de correo electrónico.

- **Respuesta de error**:
  - Código de estado: 401 (No autorizado)
  - Cuerpo de la respuesta: Inserte un usuario válido

### Token y cambio de contraseña

- **URL**: `/resent`
- **Método**: `POST`
- **Encabezados de la solicitud**:
  - `Authorization: Bearer {token}`

    | body              | Tipo   | Descripción       |
    | -----------       | ------ | ----------------- |
    | `newpassword`     | String | Email del usuario |
    | `password`        | String | Email del usuario |
- **Respuesta exitosa**:
  - Código de estado: 200 (OK)
  - Cuerpo de la respuesta: Información del usuario autenticado

- **Respuesta de error**:
  - Código de estado: 409 (No autorizado)
  - Cuerpo de la respuesta: Inserte un usuario válido

## Ejemplo de Uso

### Curl 
``` bash
# Registro de Usuario
curl -X POST -H "Content-Type: application/json" -d '{"username":"ejemplo_usuario","password":"contraseña_segura","email":"usuario@example.com"}' http://tu-api.com/register

# Inicio de Sesión
curl -X POST -H "Content-Type: application/json" -d '{"username":"ejemplo_usuario","password":"contraseña_segura"}' http://tu-api.com/login

# Obtener Información del Usuario
curl -X GET -H "Authorization: Bearer {token_de_acceso}" http://tu-api.com/user

```

### Javascript Fetch

```javascript
// Registro de Usuario
fetch('http://tu-api.com/register', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: 'ejemplo_usuario',
    password: 'contraseña_segura',
    email: 'usuario@example.com'
  })
});
// autenticacion de usuario
fetch('http://tu-api.com/verify', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    code: 'ejemplo_usuario',
  })
});

// Email de recuperacion de contraseña
fetch('http://tu-api.com/recover', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    email: 'mail', 
    })
});

// Inicio de Sesión
fetch('http://tu-api.com/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: 'ejemplo_usuario',
    password: 'contraseña_segura'
  })
});

// Obtener Información del Usuario
fetch('http://tu-api.com/user', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer {token_de_acceso}'
  }
});

// recovery password
fetch('')

```

### Python Requests

```python
    import requests

    # Registro de Usuario
    register_url = 'http://tu-api.com/register'
    register_data = {
        'username': 'ejemplo_usuario',
        'password': 'contraseña_segura',
        'email': 'usuario@example.com'
    }
    response = requests.post(register_url, json=register_data)
    print(response.json())

    # Inicio de Sesión
    login_url = 'http://tu-api.com/login'
    login_data = {
        'username': 'ejemplo_usuario',
        'password': 'contraseña_segura'
    }
    response = requests.post(login_url, json=login_data)
    print(response.json())

    # Obtener Información del Usuario
    user_url = 'http://tu-api.com/user'
    headers = {
        'Authorization': 'Bearer {token_de_acceso}'
    }
    response = requests.get(user_url, headers=headers)
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
        // Registro de Usuario
        string registerUrl = "http://tu-api.com/register";
        var registerData = new
        {
            username = "ejemplo_usuario",
            password = "contraseña_segura",
            email = "usuario@example.com"
        };
        var registerResponse = await PostAsync(registerUrl, registerData);
        Console.WriteLine(await registerResponse.Content.ReadAsStringAsync());

        // Inicio de Sesión
        string loginUrl = "http://tu-api.com/login";
        var loginData = new
        {
            username = "ejemplo_usuario",
            password = "contraseña_segura"
        };
        var loginResponse = await PostAsync(loginUrl, loginData);
        Console.WriteLine(await loginResponse.Content.ReadAsStringAsync());

        // Obtener Información del Usuario
        string userUrl = "http://tu-api.com/user";
        string token = "{token_de_acceso}";
        var userResponse = await GetAsync(userUrl, token);
        Console.WriteLine(await userResponse.Content.ReadAsStringAsync());
    }

    private static async Task<HttpResponseMessage> PostAsync(string url, object data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(data),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        return await client.SendAsync(request);
    }

    private static async Task<HttpResponseMessage> GetAsync(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }
}

```
