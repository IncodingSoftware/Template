namespace Template.UI.Models
{
    #region << Using >>

    using System.Collections.Generic;

    #endregion

    public class ComplexVm
    {
        #region Properties

        public string Group { get; set; }

        public bool IsRed { get; set; }

        public List<CountryVm> Country { get; set; }

        #endregion
    }
}