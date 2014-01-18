namespace Template.UI.Controllers
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Incoding.MvcContrib;
    using Template.UI.Models;

    #endregion

    public class DataController : IncControllerBase
    {
        #region Http action

        [HttpGet]
        public ActionResult FetchComplex()
        {
            return IncJson(new List<ComplexVm>
                               {
                                       new ComplexVm { Country = GetCountries(), Group = "Public", IsRed = true }, 
                                       new ComplexVm { Country = GetCountries(), Group = "Private", IsRed = false }, 
                               });
        }

        [HttpGet]
        public ActionResult FetchCountries()
        {
            return IncJson(GetCountries());
        }

        [HttpGet]
        public ActionResult FetchCountry()
        {
            return IncJson(GetCountries().FirstOrDefault());
        }

        [HttpGet]
        public ActionResult FetchEmpty()
        {
            return IncJson(new List<CountryVm>());
        }

        [HttpGet]
        public ActionResult FetchUnary()
        {
            return IncJson(new List<UnaryVm>
                               {
                                       new UnaryVm { Is = true }, 
                                       new UnaryVm { Is = false }, 
                                       new UnaryVm { Is = true }, 
                               });
        }

        #endregion

        List<CountryVm> GetCountries()
        {
            Func<string, string[], CountryVm> createCountry = (title, cities) =>
                                                                  {
                                                                      var country = new CountryVm { Title = title, Code = title.GetHashCode().ToString(), Cities = new List<CityVm>() };
                                                                      foreach (var city in cities)
                                                                          country.Cities.Add(new CityVm { Name = city });

                                                                      return country;
                                                                  };

            return new List<CountryVm>
                       {
                               createCountry("Afghanistan", new[] { "Kabul", "Kandahar" }), 
                               createCountry("Minsk", new[] { "Kabul", "Barysaw" }), 
                               createCountry("Cambodia", new[] { "Battambang", "Kampong Cham" }), 
                               createCountry("Denmark", new[] { "Copenhagena", "Aarhus" }), 
                               createCountry("Egypt", new[] { "Cairo", "Alexandria" }), 
                               createCountry("Greece", new[] { "Athens", "Thessaloniki" }), 
                       };
        }
    }
}