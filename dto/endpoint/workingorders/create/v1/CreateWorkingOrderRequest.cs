namespace dto.endpoint.workingorders.create.v1
{

    public class CreateWorkingOrderRequest
    {
        ///<Summary>
        ///Instrument epic
        ///</Summary>
        public string epic { get; set; }
        ///<Summary>
        ///Expiry
        ///</Summary>
        public string expiry { get; set; }
        ///<Summary>
        ///Deal direction
        ///</Summary>
        public string direction { get; set; }
        ///<Summary>
        ///Order size
        ///</Summary>
        public decimal? size { get; set; }
        ///<Summary>
        ///Deal level
        ///</Summary>
        public decimal? level { get; set; }
        ///<Summary>
        ///Deal type
        ///</Summary>
        public string type { get; set; }
        ///<Summary>
        ///Currency.  Restricted to available instrument currencies
        ///</Summary>
        public string currencyCode { get; set; }
        ///<Summary>
        ///Time in force type
        ///</Summary>
        public string timeInForce { get; set; }
        ///<Summary>
        ///Good until date
        ///</Summary>
        public string goodTillDate { get; set; }
        ///<Summary>
        ///Guaranteed stop
        ///</Summary>
        public bool guaranteedStop { get; set; }
        ///<Summary>
        ///Stop level
        ///</Summary>
        public decimal? stopLevel { get; set; }
        ///<Summary>
        ///Stop distance
        ///</Summary>
        public decimal? stopDistance { get; set; }
        ///<Summary>
        ///Limit level
        ///</Summary>
        public decimal? limitLevel { get; set; }
        ///<Summary>
        ///Limit distance
        ///</Summary>
        public decimal? limitDistance { get; set; }
    }
}
