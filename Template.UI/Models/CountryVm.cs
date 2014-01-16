namespace Template.UI.Models
{
    #region << Using >>

    using System.Collections.Generic;

    #endregion

    public class CountryVm
    {
        #region Properties

        public string Code { get; set; }

        public string Title { get; set; }
        
        public List<CityVm> Cities { get; set; }
        
        #endregion
    }
}