using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.ExceptionFacade
{
    public class CarException : Exception
    {
        //public static string descString { get; set; }

        /// <param name="description">description for where and when the Exception come from </param>
        /// <param name="e">the throwed out Exception </param>
        public CarException(string description, Exception e):
            base(String.Format("Occured a catched exception【{0} at {1} for - {2}】",e.Message +" in "+e.StackTrace, DateTime.Now.ToString(), description), e)
        {
            
        }
    }

    public class CarExceptionWithNoCar : Exception
    {
        public CarExceptionWithNoCar(uint carBusinessModelID, uint serviceProviderID, string cdCode, string vendorCode) :
            base(String.Format(CarErrorMessage.CarBSSearchNoCarIssue,carBusinessModelID,serviceProviderID,cdCode,vendorCode))
        {
            
        }
    }

    /// <summary>
    ///  The print class to control wheather print the error trace in test restul and the remote file 
    /// </summary>
    public class CarExceptionPrint
    {
        // print the failed details trace from code defect in test result. 
        //public bool printInTestResult { get; set; }

        public static string PrintExcpetion(Exception e)
        {
            // do the print in file path ...... not implement
            // if(CarExceptionPrint.printInFile)
            string returnString = e.Message;
            //// always show the failed class name and line
            //var st = new System.Diagnostics.StackTrace();

            bool needPrintInTestResult = true;
            if (needPrintInTestResult)
            {
                returnString += e.StackTrace;
            }
            //// print the failed details trace from code defect into file path.
            //public bool printInFile { get; set; }
            return returnString;
        }


    }
}
