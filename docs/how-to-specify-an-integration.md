# How To Specify an Integration

These notes should be kept in mind when writing a specification for an integration between two systems. They have all arisen from questions I've had when trying to implement an integration while relying on specifications that were invariably brief, vague, incomplete, or just badly written.

## Introduction

* Give a one-sentence description of each system being integrated.
* Describe the business problem that the integration addresses.
* Give a brief description of the integration
    * Method: HTTP, SOAP, queues, files
    * Direction: Which system exposes an API, which system consumes it?
* Walk through how the integration will work end-to-end. Particularly where the integration involves multiple steps, describe the sequence of events. Explain how this addresses the business problem.

## For Each Integration Point:

* Give the integration point a clear name.
* Identify which system is the service and which is the client. Is this integration a "push" or a "pull"?
* When is the integration point used? In response to a user action? On a schedule?
* Identify the technology/protocol used.
* Where is the integration point hosted? Give the URL/path/address, if known.
* How does the client authenticate with the serice?
* Where do credentials come from? How are they stored securely? Should they be cached?
* For a web service:
    * Give the URI and identify any parameters.
    * Specify any required request headers.
    * Specify any response headers.
* For a queue:
    * Identify the technology: Azure Service Bus, MQ
* For a directory drop:
    * Give the transfer protocol: FTP, network share
* Link to any third-party libraries or documentation that are required to use the integration point.
* How often will the integration point be called?
* What is the expected response time?
* How long should it take between a user triggering an action and seeing a result?
* How will errors be handled?

## Parameters

Wherever URL parameters, query string parameters, headers, etc are used, specify each one.

* Name
* Data type: Give each data type used a unique name, and define all the data types in one place in the specification. 
* Optionality
* Example

## Payload

For each integration point, specify the message body, request, response, or file content.

* Format: JSON, XML, CSV
* Encoding: ASCII, UTF-8, Base-64
* Is encryption used? If so, what is the algorithm? Where are keys issued from?

Specify every field. If a schema document is available, link to it, but also list every field in a table (schema documents are rarely as human-readable as they claim to be). Source and destination mappings might technically be outside the scope of the integration, but they're useful to have, and may not be documented anywhere else. A note should be included though indicating that the mappings are only correct at the time of writing - it's not necessary to try to keep them up to date.

* Name
* Data type (defined below)
* Optionality
* Source system mapping
* Destination system mapping
* Examples
* Comments

## Data Types

Define all the data types used in the specification. Don't just stick with basic types like "string" and "int"; wherever a string has a different maximum length, or a date has a different format, define a new data type.

* Name
* Description
* Representation (string, number, boolean)
* Format
* Length
* Range
* Time zone
* Enumerated values
* Examples

## Operations

Any operational requirements should be kept in mind, although they won't necessarily belong in an integration specification, since a lot of these are "implemetation details".

* How will the integration be deployed to Dev, Test, and Live environments?
* How can the integration be tested end-to-end?
* Who will be reposonsible for setting up any queues/databases/directories?
* Who will receive error reports? What actions will they take?
* What logging is required, where will the logs be stored, and for how long?
* Will messages/files be retained? For how long, and how will they be deleted?
* What settings control the integration? Where are they stored and what do they do?

