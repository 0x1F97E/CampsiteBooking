# Information Management Testing Guide

## 🎯 Purpose
This guide helps you verify that the Information Management page (`/admin/information`) and the user-facing Information page (`/information`) are correctly loading events and newsletters from the database instead of using hardcoded data.

---

## ✅ What Was Fixed

### **Admin Information Management Page (`/admin/information`)**
1. ✅ **Events** - Now loaded from `Events` table instead of hardcoded data
2. ✅ **Newsletters** - Now loaded from `Newsletters` and `NewsletterAnalytics` tables
3. ✅ **Newsletter Subscriptions** - Statistics loaded from `NewsletterSubscriptions` table
4. ✅ **Refresh Button** - Added manual refresh button to reload data
5. ✅ **Enhanced Logging** - Detailed console output showing what data is loaded
6. ✅ **Record Counts** - Page header shows event and newsletter counts

### **User-Facing Information Page (`/information`)**
1. ✅ **Events** - Already correctly loading from database
2. ✅ **Campsite Names** - Fixed to show actual campsite names instead of "Campsite {id}"
3. ✅ **Enhanced Logging** - Added detailed console output
4. ✅ **Event Filtering** - Events correctly filtered by selected campsite

---

## 🧪 Testing Steps

### **Step 1: Restart the Application**

```bash
dotnet run
```

**Expected Console Output:**
```
📰 ADMIN INFORMATION MANAGEMENT - Loading data from database...
   ✅ Loaded 5 campsites
   ✅ Loaded X active newsletter subscribers (Y new this week)
   ✅ Loaded 6 events from Events table
   ✅ Loaded X recent newsletters
   ✅ Loaded X newsletters for history table

   📋 Recent events (last 3):
      - Summer Beach Party at Copenhagen Beach Camp on Jun 15, 2025
      - Sunrise Yoga on the Beach at Copenhagen Beach Camp on May 30, 2025
      - Kids Adventure Day at Odense Family Camp on May 20, 2025

   📋 Recent newsletters (last 3):
      - Summer 2025 Special Offers (Sent) - 250 recipients
      - New Campsite Opening in Bornholm (Sent) - 180 recipients
      - Spring Cleaning Discount (Sent) - 320 recipients
```

---

### **Step 2: Verify Admin Information Management Page**

1. **Navigate to**: `http://localhost:5063/admin/information`

2. **Check the page header**:
   - Should show: "Information & Newsletter Management (6 events, X newsletters)"
   - Should have a "Refresh" button

3. **Check the Events tab**:
   - Should show 6 events from the database
   - Events should have actual campsite names (e.g., "Copenhagen Beach Camp", not "Campsite 1")
   - Events should show:
     - Summer Beach Party (Copenhagen Beach Camp)
     - Northern Lights Photography Workshop (Skagen North Point)
     - Guided Forest Hiking Tour (Aarhus Forest Retreat)
     - Kids Adventure Day (Odense Family Camp)
     - Bornholm Island Cycling Tour (Bornholm Island Camp)
     - Sunrise Yoga on the Beach (Copenhagen Beach Camp)

4. **Check the Newsletter History tab**:
   - Should show newsletters from the database
   - Should show sent dates, recipient counts, open rates, click rates

5. **Test the Refresh button**:
   - Click the "Refresh" button
   - Should see a success message: "Refreshed! Found 6 events and X newsletters"
   - Console should show the loading output again

---

### **Step 3: Verify User-Facing Information Page**

1. **Navigate to**: `http://localhost:5063/information`

2. **Check console output**:
```
📰 USER INFORMATION PAGE - Loading data from database...
   ✅ Loaded 5 campsites for Information page
   ✅ Loaded 6 events for Information page
   ✅ Loaded X photos for Information page

   📋 Events by campsite:
      - Copenhagen Beach Camp: 2 events
      - Skagen North Point: 1 events
      - Aarhus Forest Retreat: 1 events
      - Odense Family Camp: 1 events
      - Bornholm Island Camp: 1 events
```

3. **Check the Events Calendar section**:
   - Should show 6 events with actual campsite names
   - Events should be filterable by campsite using the dropdown

4. **Test campsite filtering**:
   - Select "Copenhagen Beach Camp" from the dropdown
   - Should show only 2 events (Summer Beach Party and Sunrise Yoga)
   - Select "All Campsites"
   - Should show all 6 events again

---

### **Step 4: Verify Data Synchronization**

This step verifies that events created in the admin appear on the user-facing page.

**Note**: Event creation functionality may not be fully implemented yet. If the EventDialog doesn't save to the database, this test will fail. This is expected and indicates that the EventDialog needs to be updated to save to the database.

1. **In Admin** (`/admin/information`):
   - Click "Add Event" button
   - Fill in event details
   - Save the event

2. **Check console output**:
   - Should see: "Event added successfully"
   - Should see the LoadData() output showing the new event count

3. **In User Page** (`/information`):
   - Refresh the page
   - The new event should appear in the events calendar

---

## 🔍 Troubleshooting

### **Problem: No events showing**

**Check**:
1. Database was seeded correctly
2. Run this SQL query to verify events exist:
   ```sql
   SELECT * FROM Events;
   ```
3. Check console for error messages

**Solution**:
- Delete the database and restart the application to re-seed

### **Problem: Events show "Campsite {id}" instead of campsite name**

**Check**:
- The `GetCampsiteName()` helper method in InformationManagement.razor
- The `GetCampsiteNameById()` helper method in Information.razor

**Solution**:
- Verify campsites are loaded correctly in `LoadData()` method

### **Problem: Newsletter statistics show 0**

**Check**:
- Newsletter subscriptions were seeded
- Run this SQL query:
   ```sql
   SELECT * FROM NewsletterSubscriptions WHERE IsActive = 1;
   ```

**Solution**:
- Verify `SeedNewsletterSubscriptions()` is being called in DatabaseSeeder

---

## 📊 Expected Database State

After seeding, the database should contain:

- **Events**: 6 events across 5 campsites
- **Newsletters**: 5 newsletters (3 sent, 1 scheduled, 1 draft)
- **NewsletterSubscriptions**: Multiple active subscriptions
- **NewsletterAnalytics**: Analytics for sent newsletters

---

## ✅ Success Criteria

- ✅ Admin page shows 6 events from database
- ✅ Admin page shows newsletters from database
- ✅ User page shows 6 events from database
- ✅ Events have actual campsite names (not "Campsite {id}")
- ✅ Events can be filtered by campsite on user page
- ✅ Refresh button works on admin page
- ✅ Console shows detailed loading diagnostics
- ✅ No hardcoded data is displayed

---

## 🚀 Next Steps

After verifying the above:

1. Test creating a new event via admin and verify it appears on user page
2. Test editing an event and verify changes appear on user page
3. Test deleting an event and verify it's removed from user page
4. Test newsletter creation and sending functionality

