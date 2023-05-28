using Estimmo.Shared.Utility;
using Xunit;

namespace Estimmo.Test.Utility
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
        [InlineData("Rue des 5 Chemins", "Rue des 5 Chemins")]
        [InlineData("Rue de l'ile Verte", "Rue de l'Ile Verte")]
        [InlineData("Rue mte du Pechu", "Rue Montée du Pechu")]
        [InlineData("Zi de la Brondalliere", "ZI de la Brondalliere")]
        [InlineData("Cheminement de la Zi de Pollet", "Cheminement de la ZI de Pollet")]
        public void NormaliseStreet(string input, string expected)
        {
            Assert.Equal(expected, _addressNormaliser.NormaliseStreet(input));
        }
    }
}
