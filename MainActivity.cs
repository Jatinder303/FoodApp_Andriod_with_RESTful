using Android.Widget;
using Newtonsoft.Json;
using System.Text;

namespace FoodApp_Andriod_with_RESTful
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private EditText search_Item_editText, min_Fat, min_Protein, min_Carb, min_Calories;
        private Button search_Button;
        private TextView Searched_Items_TextView;
        private Spinner dietSpinner, cuisineSpinner;
        string selectedDiet, selectCuisine;
        private const string ApiKey = "d6838312a8c94c7180e415e47b426054";
        private const string ApiUrl = "https://api.spoonacular.com/recipes/complexSearch";

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);  

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            search_Item_editText = FindViewById<EditText>(Resource.Id.SelectItem_EditText);
            min_Calories = FindViewById<EditText>(Resource.Id.min_Calories_editText);
            min_Fat = FindViewById<EditText>(Resource.Id.min_Fat_editText);
            min_Carb = FindViewById<EditText>(Resource.Id.min_carb_editText);
            min_Protein = FindViewById<EditText>(Resource.Id.min_Protien_editText);
            dietSpinner = FindViewById<Spinner>(Resource.Id.dietType_spinner);
            cuisineSpinner = FindViewById<Spinner>(Resource.Id.cuisineType_spinner);
            search_Button = FindViewById<Button>(Resource.Id.btn_Serach);
            Searched_Items_TextView = FindViewById<TextView>(Resource.Id.SearchedItems_TextView);


            dietSpinner.ItemSelected += (sender, e) =>
            {
                selectedDiet = dietSpinner.GetItemAtPosition(e.Position).ToString();
            };

            cuisineSpinner.ItemSelected += (sender, e) =>
            {
                selectCuisine = cuisineSpinner.GetItemAtPosition(e.Position).ToString();
            };
            search_Button.Click += async (sender, e) =>
            {
                string searchdata = search_Item_editText.Text;
               if (!string.IsNullOrEmpty(searchdata))
                {
                    string apiUrl = $"{ApiUrl}?apiKey={ApiKey}&query={searchdata}&diet={selectedDiet}&cuisine={selectCuisine}&minProtein={min_Protein.Text}&minCarbs={min_Carb.Text}&minCalories={min_Calories.Text}&minFat={min_Fat.Text}";
                    
                    Searched_Items_TextView.Text = await SearchRecipes(apiUrl);
                }
                else
                {
                    Searched_Items_TextView.Text = "Please Enter Recipe Name for Ssearch";
                }
            };
            Searched_Items_TextView.Click += obj_TextView_item_Selected;
        }

        private void obj_TextView_item_Selected(object? sender, EventArgs e)
        {
            string SelectedItem_Text = Searched_Items_TextView.Text;
            int id;
            if (int.TryParse(SelectedItem_Text, out id))
            {
                Toast.MakeText(this, id, ToastLength.Short).Show();
            }
            else
            {
                // Handle the case where the text is not a valid integer ID
                Toast.MakeText(this, "Invalid ID format in TextView text", ToastLength.Short).Show();
            }
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
                            StringBuilder nutriValue = new StringBuilder();
                            foreach(var nutri in recipe.nutrition.nutrients)
                            {
                                nutriValue.AppendLine(nutri.name + " : " + nutri.amount.ToString() + nutri.unit);
                            }
                            stringBuilder.AppendLine($"Recipe Id : {recipe.id} \n Recipe Name : {recipe.title}  \n" + nutriValue.ToString());

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