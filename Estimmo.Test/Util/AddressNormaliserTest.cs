using Estimmo.Shared.Util;
using Xunit;

namespace Estimmo.Test.Util
{
    public class AddressNormaliserTest
    {
        private readonly AddressNormaliser _addressNormaliser = new();

        [Theory]
        [InlineData("QUARTIER DE L HOSPICE", "Quartier de l'Hospice")]
        [InlineData("RUE DE PARIS", "Rue de Paris")]
        [InlineData("BARRY D EN HAUT", "Barry d'en Haut")]
        [InlineData("RTE DE LOUBENS", "Route de Loubens")]
        [InlineData("IMP DE LA GALAGE", "Impasse de la Galage")]
        [InlineData("Rue du  Colonel Melville Lynch", "Rue du Colonel Melville Lynch")]
        public void NormaliseStreet(string input, string expected)
        {
            Assert.Equal(expected, _addressNormaliser.NormaliseStreet(input));
        }
    }
}
