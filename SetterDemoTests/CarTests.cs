using System;
using Xunit;
using SetterDemo;
using Microsoft.EntityFrameworkCore;

namespace SetterDemoTests
{
    public class CarTests
    {
        [Fact]
        public void Car_OnOwnerChange_IncrementsOwnerCount()
        {
            var car = new Car();
            car.Owner = new Owner("owner");
            Assert.Equal(1, car.OwnerCount);
        }

        [Fact]
        public void WronglyConfiguredBackingField()
        {
            var options = new DbContextOptionsBuilder<DemoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            using (var context = new DemoContext(options))
            {
                var car = new Car();
                car.Owner = new Owner("Salesman");
                context.Cars.Add(car);
                context.SaveChanges();
            }
            using (var context = new DemoContext(options))
            {
                var client = new Client(context);
                var car = client.GetCarById(1);
                client.CarSoldToNewOwner(1, new Owner("Customer"));
                Assert.Equal(2, car.OwnerCount);
            }
        }
    }
}
