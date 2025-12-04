### Project Overview: Requirenments from Product Owner & used Technology

This repository contains the foundation for a campsite booking system, built based on the requirements gathered from the Product Owner. Below are the key user requirements for the system:

#### 1. **System Requirements:**

* **Multi-Campsite Support:** The system must support the management of bookings for **multiple campsites**. Each campsite should have independent settings for accommodation types, pricing, and availability.
* **Accommodation Types:** The system should allow the booking of various accommodation types, including cabins, tent sites, and additional options like glamping or RV spots.

#### 2. **Pricing & Seasons:**

* **Season Definition:** The system should support **seasonal pricing**, with the ability to define:

  * High Season
  * Medium Season
  * Low Season
    Each season should have configurable start and end dates, and prices should adjust automatically based on the season.
* **Price Management:** Prices should be adjustable:

  * **Automatically:** Based on season and availability.
  * **Manually:** Administrators should have the ability to override prices for special circumstances.

#### 3. **Communication:**

* **Automated Emails:** The system should automatically send emails for:

  * Booking Confirmation
  * Cancellation Confirmation
  * Reminder Emails (e.g., 24 hours before check-in)
  * Availability Alerts (if previously full spots open up).
* **SMS & Messaging:** Optional SMS notifications for bookings, cancellations, and reminders.

#### 4. **User Management & Login:**

* **User Roles:** Support different user roles:

  * **Guests:** Ability to search and book accommodations.
  * **Administrators:** Full access to manage settings, view all bookings, and adjust pricing.
  * **Staff:** Limited access to check-in/check-out and booking management.
* **Access Control:** Implement **role-based access control** (RBAC) to restrict access to specific features based on the user's role.

#### 5. **Site Map:**

* **Visual Site Map:** The system should provide an interactive map where users can:

  * View available and occupied spots
  * Select their accommodation visually
  * View accommodation types and spot sizes.
* **Real-Time Availability:** The map should display **real-time availability** and immediately update when a booking is made.







### Product Owner Requirements Specification

This section outlines the **Product Owner's specific requirements** for the campsite booking system. These requirements form the foundation of the project and guide both the technical implementation and user experience design.

#### 1. **System Scope:**

* **Multi-Campsite Support:**
  The system must support the management of bookings for **multiple campsites**. Each campsite should have independent settings for accommodation types, pricing, and availability.

* **Accommodation Types:**
  The system should allow the booking of various accommodation types, including:

  * Cabins (different sizes and types)
  * Tent sites (standard and premium)
  * Any future types of accommodations, such as glamping or RV spots, should be easy to integrate.

#### 2. **Pricing and Seasons:**

* **Season Definition:**
  The system should support **seasonal pricing**, where users can define:

  * High Season
  * Medium Season
  * Low Season
    Each season should have configurable start and end dates, and the system should automatically adjust prices based on the selected season.

* **Price Management:**

  * **Automatic Price Changes:** Prices should be able to automatically adjust based on the season and other criteria (e.g., number of available spots).
  * **Manual Price Changes:** Administrators must be able to manually adjust prices for each accommodation type, overriding automatic adjustments when necessary.

#### 3. **Communication:**

* **Automated Emails:**
  The system should automatically send the following emails:

  * **Booking Confirmation:** Sent to the guest after a successful booking.
  * **Booking Cancellation:** Sent to the guest after a cancellation request.
  * **Reminder Emails:** Sent 24 hours before arrival to remind guests of their booking.
  * **Availability Alerts:** If spots become available in a previously full campsite, interested guests should be notified.

* **SMS & Messaging:**
  The system should have the option to send **SMS notifications** for booking confirmations, cancellations, and reminders. Additional custom messages may also be sent.

#### 4. **User Management and Login:**

* **User Roles:**
  The system should support different user roles, including:

  * **Guests:** Ability to search for and book available accommodations.
  * **Administrators:** Full access to manage campsite settings, view all bookings, and adjust prices.
  * **Staff:** Limited access to booking management, including check-in/check-out functions and viewing bookings for specific dates.

* **Access Control:**
  The system must have **role-based access control (RBAC)** to restrict or grant access to specific functionalities based on the user's role. For example, only administrators can modify pricing, while guests can only view and make bookings.

#### 5. **Site Map:**

* **Visual Booking Map:**
  The system should include an **interactive map** that allows users to visually select their desired accommodation. The map should show:

  * Available and occupied spots
  * Accommodation types (e.g., cabins, tent sites)
  * Spot sizes (if applicable)
* **Real-Time Availability:**
  The map must reflect **real-time availability**. Once a guest selects a spot, it should be marked as unavailable, and other users should see this change immediately.
