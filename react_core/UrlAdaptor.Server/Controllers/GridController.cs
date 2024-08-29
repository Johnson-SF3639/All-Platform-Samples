using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.Base;
using UrlAdaptor.Server.Models;


namespace UrlAdaptor.Server.Controllers
{
    [ApiController]
    public class GridController : ControllerBase
    {
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            // Retrieve data from the data source (e.g., database)
            IEnumerable<OrdersDetails> DataSource = GetOrderData();

            DataOperations operation = new DataOperations(); // Initialize DataOperations instance

            // Handling searching operation
            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, DataManagerRequest.Search);
            }

            // Handling filtering operation
            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                foreach (var condition in DataManagerRequest.Where)
                {
                    foreach (var predicate in condition.predicates)
                    {
                        DataSource = operation.PerformFiltering(DataSource, DataManagerRequest.Where, predicate.Operator);
                    }
                }
            }

            // Handling sorting operation
            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                DataSource = operation.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }

            // Get the total count of records
            int totalRecordsCount = DataSource.Count();

            // Handling paging operation.
            if (DataManagerRequest.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, DataManagerRequest.Skip);
            }
            if (DataManagerRequest.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, DataManagerRequest.Take);
            }

            // Return data based on the request
            return new { result = DataSource, count = totalRecordsCount };
        }

        [HttpGet]
        [Route("api/[controller]")]
        public List<OrdersDetails> GetOrderData()
        {
            var data = OrdersDetails.GetAllRecords().ToList();
            return data;
        }

        /// <summary>
        /// Inserts a new data item into the data collection.
        /// </summary>
        /// <param name="newRecord">It contains the new record detail which is need to be inserted.</param>
        /// <returns>Returns void</returns>
        [HttpPost]
        [Route("api/Grid/Insert")]
        public void Insert([FromBody] CRUDModel<OrdersDetails> newRecord)
        {
            if (newRecord.Value != null)
            {
               OrdersDetails.GetAllRecords().Insert(0, newRecord.Value);
            }
        }

        /// <summary>
        /// Update a existing data item from the data collection.
        /// </summary>
        /// <param name="Order">It contains the updated record detail which is need to be updated.</param>
        /// <returns>Returns void</returns>
        [HttpPost]
        [Route("api/Grid/Update")]
        public void Update([FromBody] CRUDModel<OrdersDetails> Order)
        {
            var updatedOrder = Order.Value;
            var data = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == updatedOrder.OrderID);
            if (data != null)
            {
                // Update the existing record
                data.OrderID = updatedOrder.OrderID;
                data.CustomerID = updatedOrder.CustomerID;
                data.ShipCity = updatedOrder.ShipCity;
                data.ShipCountry = updatedOrder.ShipCountry;
                data.ShipName = updatedOrder.ShipName;
                // Update other properties similarly
            }
        }
        /// <summary>
        /// Remove a specific data item from the data collection.
        /// </summary>
        /// <param name="value">It contains the specific record detail which is need to be removed.</param>
        /// <return>Returns void</return>
        [HttpPost]
        [Route("api/Grid/Remove")]
        public void Remove([FromBody] CRUDModel<OrdersDetails> value)
        {
            int orderId = int.Parse(value.Key.ToString());
            var data = OrdersDetails.GetAllRecords().FirstOrDefault(orderData => orderData.OrderID == orderId);
            if (data != null)
            {
                // Remove the record from the data collection
                OrdersDetails.GetAllRecords().Remove(data);
            }
        }

        [HttpPost]
        [Route("api/Grid/Validate")]
        public object Validate([FromBody] CRUDModel<OrdersDetails> value)
        {

             bool isCheck = false;
             if (value.Value.OrderID == 10001) {
                isCheck = false;
             } else {
                isCheck = true;
             }

            return new JsonResult(new { result = isCheck }); // Return true if duplicate values exist, otherwise false
        }
        

        /// <summary>
        /// Perform all the CRUD operation at server-side using a single method instead of specifying separate controller action method for CRUD (insert, update and delete) operations.
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/[controller]/CrudUpdate")]
        public void CrudUpdate([FromBody] CRUDModel<OrdersDetails> request)
        {
            if (request.Action == "update")
            {
                var orderValue = request.Value;
                OrdersDetails existingRecord = OrdersDetails.GetAllRecords().Where(or => or.OrderID == orderValue.OrderID).FirstOrDefault();
                existingRecord.OrderID = orderValue.OrderID;
                existingRecord.CustomerID = orderValue.CustomerID;
                existingRecord.ShipCity = orderValue.ShipCity;
            }
            else if (request.Action == "insert")
            {
                if (request.Value != null)
                {
                   OrdersDetails.GetAllRecords().Insert(0, request.Value);
                }
            }
            else if (request.Action == "remove")
            {
                OrdersDetails.GetAllRecords().Remove(OrdersDetails.GetAllRecords().Where(or => or.OrderID == int.Parse(request.Key.ToString())).FirstOrDefault());
            }
            
        }


    //     public class CRUDModel<T> where T : class
    //     {

    //         public string? action { get; set; }
  
    //         public string? keyColumn { get; set; }
        
    //         public object? key { get; set; }
       
    //         [JsonProperty("value")]
    //         public T? value { get; set; }

    //         public List<T>? added { get; set; }
         
    //         public List<T>? changed { get; set; }
     
    //         public List<T>? deleted { get; set; }
          
    //         public IDictionary<string, object>? @params { get; set; }
    //     }
     }
}
