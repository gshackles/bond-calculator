open canopy
open FSharp.Data
open SendGrid
open SendGrid.Helpers.Mail
open System

type Bonds = CsvProvider<HasHeaders = false, Schema = "SerialNumber(string),IssueDate(string)">

[<EntryPoint>]
let main _ = 
    start chrome
    url "https://www.treasurydirect.gov/BC/SBCPrice"

    Bonds.Load("c:\\bonds.csv").Rows
    |> Seq.iter (fun bond ->
        let denomination = match bond.SerialNumber with
                           | serial when serial.StartsWith("L") -> 50
                           | serial when serial.StartsWith("C") -> 100
                           | _ -> failwith "Unknown bond denomination"
        
        "select[name=Denomination]" << string denomination
        "input[name=SerialNumber]" << bond.SerialNumber
        "input[name=IssueDate]" << bond.IssueDate
        click "input[name='btnAdd.x']"
    )

    let totals = elements "table#ta1 tr:nth-child(2) td"
    let lines = seq {
        yield sprintf "Total Value: %s" totals.[1].Text
        yield sprintf "Total Price Paid: %s" totals.[0].Text
        yield sprintf "Total Interest: %s" totals.[2].Text
        yield sprintf "YTD Interest: %s" totals.[3].Text
    } 
    let content = lines |> String.concat "<br />"

    let client = SendGridClient(Environment.GetEnvironmentVariable "SendGridApiKey")
    let emailAddress = EmailAddress("greg@gregshackles.com", "Savings Bond Calculator")

    MailHelper.CreateSingleEmail(emailAddress, emailAddress, "Savings Bond Values", content, content)
    |> client.SendEmailAsync
    |> Async.AwaitTask
    |> Async.Ignore
    |> Async.RunSynchronously

    quit()

    0