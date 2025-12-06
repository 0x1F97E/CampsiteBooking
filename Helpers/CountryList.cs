namespace CampsiteBooking.Helpers;

/// <summary>
/// Provides a comprehensive list of countries based on ISO 3166-1 standard.
/// Countries are stored as full names to match the existing database format.
/// </summary>
public static class CountryList
{
    /// <summary>
    /// Gets a list of all countries, with Nordic/European countries prioritized at the top.
    /// </summary>
    public static IReadOnlyList<string> Countries => _countries;

    private static readonly List<string> _countries = new()
    {
        // Nordic countries (prioritized for Danish campsite booking system)
        "Denmark",
        "Sweden",
        "Norway",
        "Finland",
        "Iceland",
        
        // Other European countries (common visitors)
        "Germany",
        "Netherlands",
        "Belgium",
        "United Kingdom",
        "France",
        "Poland",
        "Czech Republic",
        "Austria",
        "Switzerland",
        "Italy",
        "Spain",
        "Portugal",
        "Ireland",
        "Luxembourg",
        "Estonia",
        "Latvia",
        "Lithuania",
        
        // Rest of Europe
        "Albania",
        "Andorra",
        "Armenia",
        "Azerbaijan",
        "Belarus",
        "Bosnia and Herzegovina",
        "Bulgaria",
        "Croatia",
        "Cyprus",
        "Georgia",
        "Greece",
        "Hungary",
        "Kosovo",
        "Liechtenstein",
        "Malta",
        "Moldova",
        "Monaco",
        "Montenegro",
        "North Macedonia",
        "Romania",
        "Russia",
        "San Marino",
        "Serbia",
        "Slovakia",
        "Slovenia",
        "Turkey",
        "Ukraine",
        "Vatican City",
        
        // North America
        "United States",
        "Canada",
        "Mexico",
        
        // Central America & Caribbean
        "Bahamas",
        "Barbados",
        "Belize",
        "Costa Rica",
        "Cuba",
        "Dominican Republic",
        "El Salvador",
        "Guatemala",
        "Haiti",
        "Honduras",
        "Jamaica",
        "Nicaragua",
        "Panama",
        "Trinidad and Tobago",
        
        // South America
        "Argentina",
        "Bolivia",
        "Brazil",
        "Chile",
        "Colombia",
        "Ecuador",
        "Guyana",
        "Paraguay",
        "Peru",
        "Suriname",
        "Uruguay",
        "Venezuela",
        
        // Asia
        "Afghanistan",
        "Bangladesh",
        "Bhutan",
        "Brunei",
        "Cambodia",
        "China",
        "India",
        "Indonesia",
        "Japan",
        "Kazakhstan",
        "Kyrgyzstan",
        "Laos",
        "Malaysia",
        "Maldives",
        "Mongolia",
        "Myanmar",
        "Nepal",
        "North Korea",
        "Pakistan",
        "Philippines",
        "Singapore",
        "South Korea",
        "Sri Lanka",
        "Taiwan",
        "Tajikistan",
        "Thailand",
        "Timor-Leste",
        "Turkmenistan",
        "Uzbekistan",
        "Vietnam",
        
        // Middle East
        "Bahrain",
        "Iran",
        "Iraq",
        "Israel",
        "Jordan",
        "Kuwait",
        "Lebanon",
        "Oman",
        "Palestine",
        "Qatar",
        "Saudi Arabia",
        "Syria",
        "United Arab Emirates",
        "Yemen",
        
        // Africa
        "Algeria",
        "Angola",
        "Benin",
        "Botswana",
        "Burkina Faso",
        "Burundi",
        "Cameroon",
        "Cape Verde",
        "Central African Republic",
        "Chad",
        "Comoros",
        "Congo",
        "Democratic Republic of the Congo",
        "Djibouti",
        "Egypt",
        "Equatorial Guinea",
        "Eritrea",
        "Eswatini",
        "Ethiopia",
        "Gabon",
        "Gambia",
        "Ghana",
        "Guinea",
        "Guinea-Bissau",
        "Ivory Coast",
        "Kenya",
        "Lesotho",
        "Liberia",
        "Libya",
        "Madagascar",
        "Malawi",
        "Mali",
        "Mauritania",
        "Mauritius",
        "Morocco",
        "Mozambique",
        "Namibia",
        "Niger",
        "Nigeria",
        "Rwanda",
        "São Tomé and Príncipe",
        "Senegal",
        "Seychelles",
        "Sierra Leone",
        "Somalia",
        "South Africa",
        "South Sudan",
        "Sudan",
        "Tanzania",
        "Togo",
        "Tunisia",
        "Uganda",
        "Zambia",
        "Zimbabwe",

        // Oceania
        "Australia",
        "Fiji",
        "Kiribati",
        "Marshall Islands",
        "Micronesia",
        "Nauru",
        "New Zealand",
        "Palau",
        "Papua New Guinea",
        "Samoa",
        "Solomon Islands",
        "Tonga",
        "Tuvalu",
        "Vanuatu",

        // Other
        "Other"
    };

    /// <summary>
    /// Searches countries that match the given search text (case-insensitive).
    /// </summary>
    public static IEnumerable<string> Search(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return Countries;

        return Countries.Where(c => c.Contains(searchText, StringComparison.OrdinalIgnoreCase));
    }
}

