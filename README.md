# Integration

This project implements an integration between BlueCorp D365 Finance & Operations and a third party logistic (3PL) warehouse.

## Data flow & Components

![image](https://github.com/user-attachments/assets/ae2e984f-22d3-4e9c-94d7-6e12f432970b)

1. Sends JSON (HTTP POST)

2. Azure Function (HTTP Trigger) → Validates JSON places it in the queue

3. Azure Queue Storage → Holds messages until they are processed

4. Azure Function (Queue Trigger) → Reads the queue, checks for duplicates, converts JSON to CSV, and sends the file to SFTP

5. Azure Table Storage → Stores processed message IDs

6. SFTP Server → Receives the CSV file

7. Retry Mechanism → If the SFTP upload fails, it automatic retries 5 times or moves to a poison queue

8. Azure Function (Queue Trigger) → Reads the queue poison, delete the message and notify (email/Teams)

- Queue automatically manages processing and retries
- Table Storage prevents duplicate processing
- Easily scalable and supports distributed processing

## Time spent designing and implementing the solution
- Design: 2 hours
- Proof of concept: 4 hours
- Documentation: 1 hours

## Anything you would have done if you had more time

- Application Insights: Create a dashboard with timeline chart and more details in a list grid related to transactions
- API Management: Add policy and configure OAuth endpoint, Limits
- Azure Function (Timer): Monitor SFTP folder and notify
- Templates for all components involved
