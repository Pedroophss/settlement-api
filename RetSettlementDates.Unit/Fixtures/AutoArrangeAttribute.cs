using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixture.AutoMoq;

namespace RetSettlementDates.Unit.Fixtures
{
    public class AutoArrangeAttribute : AutoDataAttribute
    {
        public AutoArrangeAttribute()
            : base(() => CreateGlobalCustomizedFixture())
        { }

        private static Fixture CreateGlobalCustomizedFixture()
        {
            var fx = new Fixture();

            // Generates moq for each Interface on tests args
            fx.Customize(new AutoMoqCustomization());

            return fx;
        }
    }
}
