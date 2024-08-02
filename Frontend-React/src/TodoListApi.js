// new class for api calls

export class TodoListApi 
{
    #baseUrl;

    constructor()
    {
        // Using HTTP Localhost:5000 as this aligns with the Docker container.
        this.#baseUrl = 'http://localhost:5000';
    }

   CreateError(propertyName, errorMessage )
   {
        const error = new Error("Validation Error");

        error.errors =  [{ 
            propertyName: propertyName,
            errorMessage: errorMessage
        }];

        return error;
   }

   CreateErrorFromResponse(response)
   {
        const error = new Error("Validation Error");

        error.errors =  response.errors

        return error;
   }

    // Returns a JSON.
    async Get() {
        var response = await fetch(`${this.#baseUrl}/api/TodoItems`);

        if(!response.ok)
        {
            throw this.CreateErrorFromResponse(await response.json());
        }

        var response = response.json();

        console.log(response);

        return response;
    }

    async Post( descriptionString ) {
        var response = await fetch(`${this.#baseUrl}/api/TodoItems`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "description": descriptionString
            })
          });


        var body = await response.json();

        if( response.status==400)
        {
            throw this.CreateErrorFromResponse( body );
        }

        if(!response.ok)
        {
            throw this.CreateError("PostFailed","Unknown");
        }

        return true;
    }

    async MarkAsComplete( uuid ) {
        
        var response = await fetch(`${this.#baseUrl}/api/TodoItems/${uuid}/markascomplete`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            }
          });

        if(!response.ok)
        {
            throw this.CreateError("Failed","Unable to mark as complete");
        }
    }
}