using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.ExceptionFacade
{
    public class CarErrorMessage
    {
        #region common method
        public static string compareFailErrorBuilder(string mapValueDesc, string expValue, string actValue)
        {
            return string.Format("{0} is not expected, expect: {1}, actual: {2}!", mapValueDesc, expValue, actValue);
        }
        #endregion

        #region Common for car
        public const string CarBSSearchNoCarIssue = "No one expected car is returned in search response! CarBusinessModelID: {0}, "
            +"ServiceProvideID: {1}, cdCode is: {2}, vendorCode is :{3}";
                
        #endregion
        #region worldspane
        // WorldSpane ....................

        // .................................. 
        #endregion

        #region MicroNexus
        // MicroNexus ....................

        // .................................. 

        #endregion

        #region Egencia for Amadeus
        // Egencia for Amadeus ....................

        // .................................. 
        #endregion
        
        #region OMS
        // OMS ....................
        public const string OMSCancelOrRollCackException = "Occured a exception when do a cancel or rollback operation.";
        public const string OMSProcessException = "Occured a exception when do the OMS process for ";
        public const string OMSGetDetailsAfterRetrieveException = "Get CRS log for GetDetails faild after retrieve for OMS booking :";
        public const string OMSGetDetailsBeforRetrieveException = "Get CRS log for GetDetails faild before retrieve for OMS booking :";
        public const string OMSBuildVericationMessageException = "Build verification message object error :";
        // .................................. 

        #endregion

        #region Booking logging in DB
        // Booking logging in DB ....................
        public const string DataTableReflexObjectERRO = "Assert booking table in DB throwed out error when using the reflex object propertyInfo.";
        // .................................. 
        #endregion

        #region Data logging 
        // Data logging ....................

        // .................................. 
        #endregion

        #region DB  
        // Others....................
        public const string readDBTableError = "Communicated with DB occured expection when read the Table data.";
        // .................................. 
        #endregion

        #region Other
        // Other ....................
        public const string ReadXMLReserveDataError = "occured a exception when read the XML file data to test reserve data.";
        public const string ReadXMLBasicDataError   = "occured a exception when read the XML file data to Basice test config data.";
        // .................................. 

        public const string VerificationLayerError = "Occured a error form veficaiton layer";

        public const string FinancialDetails = "FinancialDetails";
        public const string FinancialSummary = "FinancialSummary";
        #endregion

        #region CurrencyNotAvailableError
        public const string CurrencyNotAvailableError_type = "CurrencyNotAvailableError";
        public const string CurrencyNotAvailableError_unabaleBook_type = "CurrencyNotAvailableError";
        public const string CurrencyNotAvailableError_message_SCS = "Invalid Currency Code";
        public const string CurrencyNotAvailableError_to_Reserve = "Invalid Currency Code";
        #endregion

        #region CarBS CarMIPSearch
        public const string CarBS_packageBooleanMissError = "CarECommerceSearchStrategy/PackageBoolean was missing but is required";
        public const string CarBS_specifyPuchaseTypeAndCategoryCodeError = "Cannot specify both PurchaseTypeMask and ProductCategoryCodeList";
        public const string CarBS_productTokenNotSupportError = "Searching by just the PostPurchaseBoolean is currently not supported";
        #endregion
    }
}
