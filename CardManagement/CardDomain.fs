﻿namespace CardManagement

module CardDomain =

    open System.Text.RegularExpressions
    open CardManagement.Common.Errors
    open CardManagement.Common

    let cardNumberRegex = new Regex("^[0-9]{16}$", RegexOptions.Compiled)

    type CardNumber = private CardNumber of string
        with
        member this.Value = match this with CardNumber s -> s
        static member create fieldName str =
            match str with
            | (null|"") -> validationError fieldName "card number can't be empty"
            | str ->
                if cardNumberRegex.IsMatch(str) then CardNumber str |> Ok
                else validationError fieldName "Card number must be of 16 digits only"

    [<Struct>]
    type DailyLimit =
        private
        | Limit of decimal
        | Unlimited
        with
        static member ofDecimal dec =
            if dec > 0m then Limit dec
            else Unlimited

    let (|Limit|Unlimited|) limit =
        match limit with
        | Limit dec -> Limit dec
        | Unlimited -> Unlimited

    type BasicCardInfo =
        { Number: CardNumber
          Name: LetterString
          Expiration: (Month*Year) }

    type UserId = System.Guid

    type CardInfo =
        { BasicInfo: BasicCardInfo
          Balance: Money
          DailyLimit: DailyLimit
          Holder: UserId }

    type Card =
        | Active of CardInfo
        | Deactivated of BasicCardInfo
        with
        member this.Number =
            match this with
            | Active card -> card.BasicInfo.Number
            | Deactivated card -> card.Number

    type CardDetails =
        { Card: Card 
          HolderAddress: Address
          HolderName: LetterString }

    type User =
        { Name: LetterString
          Id: UserId
          Address: Address
          Cards: Card list }