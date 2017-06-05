# Municipality Tax Schedule
**A small C# coding assignment**

Author: Keith Hall

## Specification

To create a small application which manages taxes applied in different municipalities.

The taxes are scheduled in time. The application should provide the user an ability to get taxes applied in a certain municipality on any given day.

Example: Municipality "Vilnius" has its taxes scheduled like this:
- yearly tax = 0.2 (for period 2016.01.01-2016.12.31),
- monthly tax = 0.4 (for period 2016.05.01-2016.05.31),
- it has no weekly taxes scheduled,
- and it has two daily taxes scheduled = 0.1 (at days 2016.01.01 and 2016.12.25).

The result according to provided example would be:

| Municipality (Input) | Date (Input) | Result |
| -------------------- | ------------ | ------ |
| Vilnius              | 2016.01.01   | 0.1    |
| Vilnius              | 2016.05.02   | 0.4    |
| Vilnius              | 2016.07.10   | 0.2    |
| Vilnius              | 2016.03.16   | 0.2    |

Full requirements for the application are:
(choose the priority of tasks in the way that when the time ends up you would have a working application, not necessarily with all functionality):

* It has its own database where municipality taxes are stored
* Taxes should have the ability to be scheduled (yearly, monthly, weekly, daily) for each municipality
* The application should have the ability to import municipalities data from a file (choose one data format you believe is suitable)
* The application should have ability to insert new records for municipality taxes (one record at a
time)
* The user can ask for a specific municipality tax by entering the municipality name and a date
* Errors need to be handled i.e. internal errors should not to be exposed to the end user
* You should ensure that application works correctly

The application has no visible user interface, requests are given directly to application as a service
(producer service). Also, there should be a consumer service created to demonstrate how the
producer service can be used.

Bonus tasks (if there is time left):

* Application is deployed as a self-hosted Windows service
* Update record functionality is exposed via API

## Solution

My original implemention wouldn't allow:

- a Yearly schedule to start not on the first day of January
- a Monthly schedule to start not on the 1st of the month
- a Weekly schedule to start not on a Monday

I since realised that maybe that's a constraint that I assumed, as it is not specifically mentioned in the specification, so I decided to add another Tax Validation class that would remove this restriction, and enable it to be configured in the config file.

The relevant configuration setting is in the `AppSettings` section, with key `ITaxScheduleValidator`, and currently 2 possible values:

- `MunicipalityTaxes.PermissiveDateTaxScheduleValidator` for a less restrictive schedule date validator
- `MunicipalityTaxes.TaxScheduleValidator` for a more restrictive schedule date validator

Also, the task mentioned that the solution should have it's own database - for simplicity I chose to interpret this loosely, and am using an "in memory database" which could easily be swapped out for another `ITaxStorageProvider` if one was implemented, again using `AppSettings` and you guessed it, the key `ITaxStorageProvider`.

### Windows Service

Instructions on how to install a Windows Service: https://msdn.microsoft.com/en-us/library/zt39148a(v=vs.110).aspx#BK_Install
The startup type for this MunicipalityTaxes Windows Service is set to Manual by default, so you'll need to start it manually after installing. :)

The URL for the service is set in the config, and is currently http://localhost:8733/Design_Time_Addresses/MunicipalityTaxes/Service1/mex, as defined at https://github.com/keith-hall/Showcase_CSharp_MunicipalityTaxSchedule/blob/4cda15a87b536a346af33f62e035f646b7bc03ae/MunicipalityTaxes/MunicipalityTaxes/App.config#L25.
If you get an `System.ServiceModel.AddressAccessDeniedException` when you try to host the WCF service, like:

> Please try changing the HTTP port to 8733 or running as Administrator.

Then you can follow the advice at https://stackoverflow.com/a/23781805/4473405 to use `netsh http add urlacl` (as Administrator) with the relevant parameters to ensure you won't need admin privileges to host the service.

### Bulk Import

The bulk import method expects a text file in the following format:

```
Vilnius|Yearly|2016-01-01|0.2
Vilnius|Monthly|2016-05-01|0.4
Vilnius|Daily|2016-01-01|0.1
Vilnius|Daily|2016-12-25|0.1
```

i.e. for each tax schedule:

- Muncipality
- followed by a pipe character
- followed by the frequency - either "Yearly", "Monthly", "Weekly" or "Daily"
- followed by a pipe character
- followed by a date that .NET can parse unambiguously
- followed by a pipe character
- followed by the tax amount (a double that .NET can parse)
- followed by a new line character (CRLF in Windows world)

Note that, when calling the web service manually, WCF Test Client expects .NET format strings i.e. instead of `C:\Users\Keith\Downloads\Vilnius.txt`, it wants `C:\\Users\\Keith\\Downloads\\Vilnius.txt`

It has been designed so that as long as it parses successfully, even if there are failures when inserting the records, it will show the status of each line item. Although, it is worth noting that the first unexpected failure (i.e. not just that the tax record already exists) will cause it to stop attempting to insert the other records.

To give an example, this can be seen when using a file with the following contents (note the duplication of the yearly schedule):

```
Vilnius|Yearly|2016-01-01|0.2
Vilnius|Yearly|2016-01-01|0.2
Vilnius|Monthly|2016-05-01|0.4
Vilnius|Daily|2016-01-01|0.1
Vilnius|Daily|2016-12-25|0.1
```

the response is (which shows that line 2 was not inserted, but the others were validated and inserted successfully):

```xml
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
  <s:Header />
  <s:Body>
    <InsertTaxScheduleDetailsFromFileResponse xmlns="http://tempuri.org/">
      <InsertTaxScheduleDetailsFromFileResult xmlns:a="http://schemas.datacontract.org/2004/07/MunicipalityTaxes" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
        <a:Status>Success</a:Status>
        <a:lineItems xmlns:b="http://schemas.datacontract.org/2004/07/System.Collections.Generic">
          <b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
            <b:key>
              <a:MunicipalitySchedule>
                <a:Municipality>Vilnius</a:Municipality>
                <a:ScheduleBeginDate>2016-01-01T00:00:00</a:ScheduleBeginDate>
                <a:ScheduleType>Yearly</a:ScheduleType>
              </a:MunicipalitySchedule>
              <a:TaxAmount>0.2</a:TaxAmount>
            </b:key>
            <b:value>
              <a:ActionResult>Success</a:ActionResult>
              <a:Validity>Valid</a:Validity>
            </b:value>
          </b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
          <b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
            <b:key>
              <a:MunicipalitySchedule>
                <a:Municipality>Vilnius</a:Municipality>
                <a:ScheduleBeginDate>2016-01-01T00:00:00</a:ScheduleBeginDate>
                <a:ScheduleType>Yearly</a:ScheduleType>
              </a:MunicipalitySchedule>
              <a:TaxAmount>0.2</a:TaxAmount>
            </b:key>
            <b:value>
              <a:ActionResult>TaxScheduleAlreadyExists</a:ActionResult>
              <a:Validity>Valid</a:Validity>
            </b:value>
          </b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
          <b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
            <b:key>
              <a:MunicipalitySchedule>
                <a:Municipality>Vilnius</a:Municipality>
                <a:ScheduleBeginDate>2016-05-01T00:00:00</a:ScheduleBeginDate>
                <a:ScheduleType>Monthly</a:ScheduleType>
              </a:MunicipalitySchedule>
              <a:TaxAmount>0.4</a:TaxAmount>
            </b:key>
            <b:value>
              <a:ActionResult>Success</a:ActionResult>
              <a:Validity>Valid</a:Validity>
            </b:value>
          </b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
          <b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
            <b:key>
              <a:MunicipalitySchedule>
                <a:Municipality>Vilnius</a:Municipality>
                <a:ScheduleBeginDate>2016-01-01T00:00:00</a:ScheduleBeginDate>
                <a:ScheduleType>Daily</a:ScheduleType>
              </a:MunicipalitySchedule>
              <a:TaxAmount>0.1</a:TaxAmount>
            </b:key>
            <b:value>
              <a:ActionResult>Success</a:ActionResult>
              <a:Validity>Valid</a:Validity>
            </b:value>
          </b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
          <b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
            <b:key>
              <a:MunicipalitySchedule>
                <a:Municipality>Vilnius</a:Municipality>
                <a:ScheduleBeginDate>2016-12-25T00:00:00</a:ScheduleBeginDate>
                <a:ScheduleType>Daily</a:ScheduleType>
              </a:MunicipalitySchedule>
              <a:TaxAmount>0.1</a:TaxAmount>
            </b:key>
            <b:value>
              <a:ActionResult>Success</a:ActionResult>
              <a:Validity>Valid</a:Validity>
            </b:value>
          </b:KeyValuePairOfMunicipalityTaxDetailsTaxScheduleActionResultOfTaxScheduleInsertionResult72mr6BFgJfw4w7nW>
        </a:lineItems>
      </InsertTaxScheduleDetailsFromFileResult>
    </InsertTaxScheduleDetailsFromFileResponse>
  </s:Body>
</s:Envelope>
```

Of course, in keeping with the requirements, any internal errors like the specifics of parse failures etc. are not shown to the user, but are logged for the service administrators to peruse.
