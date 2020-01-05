using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SetterDemo
{
    public class Car
    {
        private Owner _owner;
        public int Id { get; set; }
        public int OwnerCount { get; private set; }
        public Owner Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                OwnerCount++;
                _owner = value;
            }
        }

        public override string ToString()
        {
            return "Owner: " + Owner.Name + " Owner count: " + OwnerCount;
        }
    }

    public class Owner
    {
        public Owner(string name)
        {
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Client
    {
        private DemoContext _context;
        public Client(DemoContext context)
        {
            _context = context;
        }
        public void CarSoldToNewOwner(int carId, Owner newOwner)
        {
            var car = GetCarById(carId);
            Console.WriteLine(car.ToString()); // owner count 2, should be 1
            car.Owner = newOwner;
            Console.WriteLine(car.ToString()); // owner count 3, should be 2
        }

        public Car GetCarById(int id)
        {
            return _context.Cars
            .Include(x => x.Owner)
            .FirstOrDefault(x => x.Id == id);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<DemoContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
            using (var context = new DemoContext(options))
            {
                var car = new Car();
                car.Owner = new Owner("Salesman"); // owner count 1
                context.Cars.Add(car);
                Console.WriteLine(car.ToString());
                context.SaveChanges();
            }
            using (var context = new DemoContext(options))
            {
                var client = new Client(context);
                var car = client.GetCarById(1);
                client.CarSoldToNewOwner(1, new Owner("Customer"));
            }
        }
    }

    public class DemoContext : DbContext
    {
        public DemoContext()
        { }
        public DemoContext(DbContextOptions<DemoContext> options) : base(options)
        { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Owner> Owners { get; set; }

        // Uncomment to make tests pass
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     var navigation = modelBuilder.Entity<Car>()
        //         .Metadata.FindNavigation(nameof(Car.Owner));

        //     navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        // }
    }
}
