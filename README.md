implementing this https://github.com/shimond/MX_EX/blob/master/README.md excerise


Service Ticket System – Functional Documentation
Overview

The Service Ticket System provides a complete workflow for creating, managing, and tracking customer service requests. It supports both public ticket creation and authenticated user operations, ensuring accessibility while maintaining proper security controls.

1. Ticket Creation (Public Access)
<img width="939" height="615" alt="image" src="https://github.com/user-attachments/assets/e691905d-e058-4d32-a046-d1df4aceba3c" />

The system allows any user — including those who are not authenticated — to open a new service ticket.

A public user can:

Submit a new ticket

Provide contact details

Describe the issue in free text

This feature ensures that customers can report problems even without having an account in advance.

2. Authenticated User Features

Users who log in to the system gain access to additional capabilities:

Viewing Tickets

Authenticated users can access a personal table that lists all their service tickets, including:

Ticket number

Current status

Date of creation

Updates from the support team

Editing Tickets

A logged-in user may edit existing tickets they created, allowing them to add information or correct details.

Registration and Login
<img width="939" height="615" alt="image" src="https://github.com/user-attachments/assets/f53994c5-b74a-46ea-adc5-cbfb69e4dff3" />
<img width="946" height="644" alt="image" src="https://github.com/user-attachments/assets/b8503e8e-b3be-45ff-b7ac-3ea7caf30011" />

The system includes:

User registration

Login and authentication

Secured access to the ticket table and ticket details
<img width="949" height="634" alt="image" src="https://github.com/user-attachments/assets/682e1bde-57dd-4a53-a911-b795e8db82ac" />

3. Email Notifications

When a user submits a service ticket, the system automatically sends a confirmation email.
The email includes:

The ticket ID

A link that allows the user to view the ticket status

Access to the treatment history and progress of the support process

This ensures transparency and real-time updates for the customer.

4. AI-Based Issue Summarization

The issue description submitted by the customer is processed using the Mistral AI model.

The system:

Sends the user’s description to the AI model

Receives a summarized version of the issue

Stores the summary together with the original description

This allows support teams to quickly understand the essence of the problem and respond more efficiently.




