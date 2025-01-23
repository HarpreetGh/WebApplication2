using Microsoft.AspNetCore.Http.HttpResults; //need to import this

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) => {
    Console.WriteLine($"Path: {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});

List<Person> people = [new Person { Name = "Tony", Age = 20}];

app.MapGet("/", () => "Hello World!");
app.MapGet("/people", () => people);
app.MapGet("/person/{id:int:min(0)}", (int id) => id < people.Count ? Results.Ok(people[id]) : Results.NotFound(id));
app.MapPut("/person/{id:int:min(0)}", (int id, Person p) => {
    if (id < people.Count) {
        people[id] = p;
        return Results.Accepted($"Person Id: {id} updated");
    }
    else {
        return Results.NotFound("Person not found");
    }
});
app.MapPost("/addPerson", (Person p) => {
    people.Add(p);
    return Results.Accepted($"Person {p.Name} added");
});

// import needed
app.MapDelete("/person/{id:int}", Results<Ok<string>, NotFound> (int id) => {
    if (0 <= id && id < people.Count) {
        people.RemoveAt(id);
        return TypedResults.Ok($"Person Id: {id} removed");
    }
    else {
        return TypedResults.NotFound();
    }
}).WithOpenApi(op => {
    op.Parameters[0].Description = "Index of array, must be valid index";
    op.Summary = "Delete Person";
    op.Description = "Delete a person via index from list of people.";
    return op;
});


app.MapGet("/test", () => { throw new Exception("Test"); }).ExcludeFromDescription();

app.Run();

public class Person {
    public string Name { get; set;}
    public int Age { get; set;}
}