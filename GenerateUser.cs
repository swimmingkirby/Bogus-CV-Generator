using Bogus;

class GenerateUser
{
    public string User()
    {
        // Generate fake person and job data using the Bogus library
        var faker = new Faker();
        var person = new
        {
            FullName = faker.Name.FullName(),
            Email = faker.Internet.Email(),
            Phone = faker.Phone.PhoneNumber(),
            Address = faker.Address.FullAddress(),
            
        };
        var job = new
        {
            Company = faker.Company.CompanyName(),
            JobTitle = faker.Name.JobTitle(),
            JobArea = faker.Name.JobArea(),
            JobType = faker.Name.JobType(),
        };

        // Construct a string containing the generated user details
        var userString = $"Name: {person.FullName}\nEmail: {person.Email}\nPhone: {person.Phone}\n FullAddress: {person.Address},\nCompany: {job.Company}\nJob Title: {job.JobTitle}\nJob Expertise: {job.JobArea}\nJob Type: {job.JobType}";
       // Console.WriteLine("Generated user string: {0}", userString);

        return userString;
    }
}
