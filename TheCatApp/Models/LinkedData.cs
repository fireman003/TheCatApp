using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCatApp.Models 
{
    public record struct Weight(string imperial, string metric);
    public record struct TheCatInfo(Weight weight, string id, string name, string cfa_url, string vetstreet_url,
        string vcahospitals_url, string temperament, string origin, string country_codes, string country_code,
        string description, string life_span, int indoor, int lap, string alt_names, int adaptability, int affection_level,
        int child_friendly, int dog_friendly, int energy_level, int grooming, int health_issues, int intelligence, int shedding_level,
        int social_needs, int stranger_friendly, int vocalisation, int experimental, int hairless, int natural, int rare, int rex,
        int suppressed_tail, int short_legs, string wikipedia_url, int hypoallergenic, string reference_image_id);

    public record struct getImageUrl(string id, string url, int height, int weight);


    internal class LinkedData : MainPage
    {
        public List<string> getImageUrl { get; set; }
        public List<TheCatInfo> TheCatInfo { get; set; }

        public LinkedData(List<string> getImageUrl, List<TheCatInfo> theCatInfo)
        {
            this.getImageUrl = getImageUrl;
            TheCatInfo = theCatInfo;
        }
    }
}
