// new class for api calls

export class TodoListApi 
{
    constructor()
    {

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

   CreateErrorFromRepsonse(response)
   {
        const error = new Error("Validation Error");

        error.errors =  response.errors

        return error;
   }

    // Returns a JSON.
    async Get() {
        var response = await fetch('https://localhost:5001/api/TodoItems');

        if(!response.ok)
        {
            throw this.CreateErrorFromRepsonse(await response.json());
        }

        var response = response.json();

        console.log(response);

        return response;
    }

    async Post( descriptionString ) {
        var response = await fetch('https://localhost:5001/api/TodoItems', {
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
            throw this.CreateErrorFromRepsonse( body );
        }

        if(!response.ok)
        {
            throw this.CreateError("PostFailed","Unknown");
        }

        return true;
    }

    async MarkAsComplete( uuid ) {
        
        var response = await fetch(`https://localhost:5001/api/TodoItems/${uuid}/markascomplete`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            }
          });

        if(!response.ok)
        {
            throw new Error("Failed","Unable to mark as complete");
        }
    }
}