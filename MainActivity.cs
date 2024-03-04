using Newtonsoft.Json;
using System.Text;

namespace FoodApp_Andriod_with_RESTful
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private EditText search_Item_editText, search_ItemDiet_editText, SelectItem_Protien_EditText;
        private Button search_Button;
        private TextView Searched_Items_TextView;

        private const string ApiKey = "d6838312a8c94c7180e415e47b426054";
        private const string ApiUrl = "https://api.spoonacular.com/recipes/complexSearch";

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);  

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            search_Item_editText = FindViewById<EditText>(Resource.Id.SelectItem_EditText);
            search_ItemDiet_editText = FindViewById<EditText>(Resource.Id.SelectItem_Diet_EditText);
            SelectItem_Protien_EditText = FindViewById<EditText>(Resource.Id.SelectItem_Protien_EditText);
            search_Button = FindViewById<Button>(Resource.Id.btn_Serach);
            Searched_Items_TextView = FindViewById<TextView>(Resource.Id.SearchedItems_TextView);

            search_Button.Click += async (sender, e) =>
            {
                string searchdata = search_Item_editText.Text;
                string search_Diet_date = search_ItemDiet_editText.Text;
                if (!string.IsNullOrEmpty(searchdata))
                {
                    string apiUrl = $"{ApiUrl}?apiKey={ApiKey}&query={searchdata}&diet={search_Diet_date}&&minProtein7={SelectItem_Protien_EditText}";
                    
                    Searched_Items_TextView.Text = await SearchRecipes(apiUrl);
                }
                else
                {
                    Searched_Items_TextView.Text = "Please enter Search Items and Diet type";
                }

            };
        }

        private async Task<string> SearchRecipes(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(apiUrl);
                try
                {
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        string content = await httpResponseMessage.Content.ReadAsStringAsync();
                        var recipes = JsonConvert.DeserializeObject<Root>(content);

                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var recipe in recipes.results)
                        {
                            stringBuilder.AppendLine($"Recipe Title: {recipe.title}");

                        }
                        return stringBuilder.ToString();
                    }
                    else
                    {
                        return $"Error:{httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error:{ex.Message}";
                }
            }
        }

    }
    public class Nutrient
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("amount")]
        public double amount { get; set; }
        [JsonProperty("unit")]
        public string unit { get; set; }
    }

    public class Nutrition
    {
        [JsonProperty("nutrients")]
        public List<Nutrient> nutrients { get; set; }
    }

    public class Result
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonProperty("image")]
        public string image { get; set; }
        [JsonProperty("imageType")]
        public string imageType { get; set; }
        [JsonProperty("nutrition")]
        public Nutrition nutrition { get; set; }
    }

    public class Root
    {
        [JsonProperty("results")]
        public List<Result> results { get; set; }
        [JsonProperty("offset")]
        public int offset { get; set; }
        [JsonProperty("number")]
        public int number { get; set; }
        [JsonProperty("totalResults")]
        public int totalResults { get; set; }
    }

}