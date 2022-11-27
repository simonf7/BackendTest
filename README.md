## BackendTest

Simple API built with .Net 6.

From the root folder build the project using:

`dotnet build`

Then run the project with:

`dotnet run`

The API server is hosted on localhost, using port 7114 providing the three endpoints
list below. They are also listed in a Postman collection within the root folder.

### POST https://localhost:7114/api/User

#### Add new a user.

Request body (form-data)

> **Name** _- Name of the user_
>
> **Email** _- Email of the user_
>
> **Password** _- Password of the user_

Returns

201 on creation of a user.

400 if there's a failure.

### POST https://localhost:7114/api/User/Login

#### Authenticate a user.

Request body (form-data)

> **Email** _- Email of the user_
>
> **Password** _- Password of the user_

Returns

201 on successful validation of a user. Response body:

```
{
    "token": "javascript.web.token"
}
```

401 unauthorized if the user can't be validated.

### GET https://localhost:7114/api/User

#### Returns a list of registered users.

The JWT returned by the authentication endpoint should be used as a Bearer Token to authorise access to the list.

Returns

200 on successful authorisation. Response body:

```
[
    {
        "id": 1,
        "name": "Simon",
        "email": "simon@example.com"
    },
    {
        "id": 2,
        "name": "Tom",
        "email": "tom@example.com"
    }
]
```

400 Bad request if the Bearer Token is missing.

401 Unauthorized if the token can't be validated.
