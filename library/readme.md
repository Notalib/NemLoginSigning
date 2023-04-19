# NemLogin3 .NET SignSDK

This project is a copy of the official NemLog-In 3 Service Provider Signing SDK.
The library is organized into a set of folders and projects all included in the SignSdk.Net solution file which can be opened with Visual Studio.

# Disclaimer
This project may be used by Service Providers to start working on NemLog-In Signing Service integration.  
However, it should be considered a prerelease version, since the backend NemLog-In services and Singing Client have not yet been made public.  
Specifically, the example webapp, included in the project, has been configured with:   
*	URLs for internal backend NemLog-In services in lieu of a publicly available API.  
*	A keystore containing dummy key material used for JWS-sealing signature parameters.
Hence, whereas the example webapp source code will still serve to demonstrate the usage of SignSDK, the application will not presently function correctly.  

# Library Structure

SDK

| Project                           	| Description |
|---------------------------------------|-------------|
| NemLoginSigningService    			| Main entry point for using the NemLogin3 .Net SignSDK. Contains only one interface and one class that implements ProduceSigningPayload.	|
| NemLoginSigningCore      				| Contains core models and service definitions. 																							|
| NemLoginSigningPades					| Project for handling pre-signing of PAdES. 																								|
| NemLoginSigningXades					| Project for handling pre-signing of Xades. 																								|
| NemLoginSigningValidation				| Project for handling validation of input before pre-signing. The project consists of validation of HTML, XML, Plain Text and PDF. 		|
| NemLoginSignatureValidationService	| Project that wraps the API request to the Signing ValidationService.																		|

# Demo Application

| Project                              	| Description |
|---------------------------------------|-----------------------------------------------------------|
| NemLoginSigningWebApp    				| Example Demo Application written in ASP .Net Core MVC.	|

# Test Project

| Project                    	| Description |
|-------------------------------|-------------|
| NemLoginSigningTest           | Test Project with relevant tests showing how to use the nemlogin3 library.	|


External Dependencies Sourcecode included

library/

| Project                  	        | Description |
|-----------------------------------|-------------------------------------------------------------------------------|
| HtmlAgilityPack.NETStandard2_0    | HtmlAgilityPack sourcecode https://github.com/zzzprojects/html-agility-pack	|
| itextsharp-netstandard    		| iTextsharp port of two projects, "iTextsharp" and "iTextsharp.XmlWorker" to .Net Standard. Original sourcecode https://github.com/itext/itextsharp	|

## Changelog

### Version 1.0.4
internal release changes - no changes to sign sdk. 

### Version 0.0.4
* rearranged projects to new grouping structure (library,test,samples)
* Updated internal IdP test certificate used by both sample applications from:  
```"login data v/kim nyhjem - tu 1"```
to: 
```"nemlog-in signsdk demo !0028funktionscertifikat!0029-fe9eac67-356c-40dc-8623-502e39bb64a5)"```
* Entity-id changed from "https://saml.serviceprovider.dk/login" to: "https://signsdk-demo.nemlog-in.dk"

### Version 0.0.3
* initial release (before completed CTI environment)

### Version 0.0.2
* internal release

### Version 0.0.1
* Initial release (internal)
