# StackDemo — Manual Test Cases
> Site: https://testathon.live
> Test accounts use dropdown selection (not free-text). All accounts share password **testingisfun99**.

| Username | Purpose |
|---|---|
| demouser | Standard test user |
| image_not_loading_user | Validates broken image handling |
| existing_orders_user | Pre-seeded order history |
| fav_user | Pre-seeded favourites |
| locked_user | Locked account scenario |

---

## Table of Contents
1. [Sign In](#1-sign-in)
2. [Home & Product Listing](#2-home--product-listing)
3. [Product Detail & Add to Cart](#3-product-detail--add-to-cart)
4. [Favourites](#4-favourites)
5. [Cart](#5-cart)
6. [Checkout](#6-checkout)
7. [Order Confirmation](#7-order-confirmation)
8. [Orders](#8-orders)
9. [Offers](#9-offers)
10. [Navigation & Header](#10-navigation--header)
11. [Exploratory Testing](#11-exploratory-testing)

---

## 1. Sign In

### TC-SI-001 — Successful sign in with valid credentials
**Priority:** High
**Preconditions:** User is on `/signin`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Select `demouser` from the Username dropdown | Username field shows `demouser` |
| 2 | Select `testingisfun99` from the Password dropdown | Password field shows selection |
| 3 | Click **Log In** | User is redirected to the home page |
| 4 | Verify the header | Logged-in user name is visible in the header |

---

### TC-SI-002 — Sign in redirects to checkout when coming from cart
**Priority:** High
**Preconditions:** User is not logged in and navigates to `/checkout`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate directly to `/checkout` | User is redirected to `/signin?checkout=true` |
| 2 | Select `demouser` and `testingisfun99` | Fields populated |
| 3 | Click **Log In** | User is redirected back to `/checkout`, not home |

---

### TC-SI-003 — Sign in redirects to favourites when coming from favourites
**Priority:** Medium
**Preconditions:** User is not logged in and navigates to `/favourites`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate directly to `/favourites` | User is redirected to `/signin?favourites=true` |
| 2 | Sign in with `demouser` | User is redirected back to `/favourites` |

---

### TC-SI-004 — Sign in redirects to offers when coming from offers
**Priority:** Medium
**Preconditions:** User is not logged in and navigates to `/offers`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate directly to `/offers` | User is redirected to `/signin?offers=true` |
| 2 | Sign in with `demouser` | User is redirected back to `/offers` |

---

### TC-SI-005 — Locked user cannot sign in
**Priority:** High
**Preconditions:** User is on `/signin`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Select `locked_user` from Username dropdown | Field shows `locked_user` |
| 2 | Select `testingisfun99` from Password dropdown | Field shows selection |
| 3 | Click **Log In** | An error message is displayed; user is NOT redirected |
| 4 | Verify the user remains on `/signin` | URL stays at `/signin` |

---

### TC-SI-006 — Username dropdown displays all expected options
**Priority:** Medium
**Preconditions:** User is on `/signin`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click the Username dropdown | Dropdown opens |
| 2 | Verify all options are listed | Options include: `demouser`, `image_not_loading_user`, `existing_orders_user`, `fav_user`, `locked_user` |
| 3 | Verify no extra unexpected options exist | Only the 5 known users are shown |

---

### TC-SI-007 — Sign In page does not allow direct text entry in Username
**Priority:** Medium
**Preconditions:** User is on `/signin`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click on the Username field | Dropdown opens (not a free-text input) |
| 2 | Attempt to type a custom username | Either text is ignored or typing filters the dropdown |
| 3 | Verify no arbitrary credential can be submitted | Only valid dropdown options can be selected |

---

### TC-SI-008 — Log In button with no credentials selected
**Priority:** High
**Preconditions:** User is on `/signin`, no credentials selected

| Step | Action | Expected Result |
|---|---|---|
| 1 | Leave both Username and Password dropdowns unselected | Both show placeholder text |
| 2 | Click **Log In** | An error message is displayed OR the button is disabled |
| 3 | Verify user is not redirected | User remains on `/signin` |

---

### TC-SI-009 — Sign in with username selected but no password
**Priority:** High
**Preconditions:** User is on `/signin`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Select `demouser` from Username | Field populated |
| 2 | Leave Password unselected | Password shows placeholder |
| 3 | Click **Log In** | Error displayed; sign in does not proceed |

---

### TC-SI-010 — Sign in page is accessible without authentication
**Priority:** Medium
**Preconditions:** None

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/signin` while not logged in | Sign in page loads without redirect |
| 2 | Navigate to `/signin` while already logged in | User is redirected away (e.g. to home) OR page still shows |

---

## 2. Home & Product Listing

### TC-PL-001 — Home page loads with product listings
**Priority:** High
**Preconditions:** User is signed in as `demouser`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/` | Page loads successfully |
| 2 | Verify products are displayed | At least one product card is visible |
| 3 | Verify each product card shows an image, name, and price | All three elements present per card |

---

### TC-PL-002 — Product images load correctly for demouser
**Priority:** High
**Preconditions:** User is signed in as `demouser`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to the home page | Products are visible |
| 2 | Check every product image | All images load without broken image icons |

---

### TC-PL-003 — Product images fail to load for image_not_loading_user
**Priority:** High
**Preconditions:** User is signed in as `image_not_loading_user`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to the home page | Products are listed |
| 2 | Check product images | Images are broken or show a placeholder/error state |

---

### TC-PL-004 — Filter products by category
**Priority:** High
**Preconditions:** User is signed in, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Locate the category filter | Filter options are visible |
| 2 | Select a specific category (e.g. a brand or product type) | Product list updates to show only items matching that category |
| 3 | Verify no products from other categories are shown | All visible products belong to the selected category |
| 4 | Clear or deselect the filter | All products are shown again |

---

### TC-PL-005 — Filter by multiple categories simultaneously
**Priority:** Medium
**Preconditions:** User is signed in, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Select the first category filter | List filters to first category |
| 2 | Select an additional category filter | List updates to show products from both categories |
| 3 | Verify products from non-selected categories are hidden | Only selected categories visible |

---

### TC-PL-006 — Sort products by price (low to high)
**Priority:** High
**Preconditions:** User is signed in, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Locate the sort/order control | Sort options visible |
| 2 | Select "Price: Low to High" | Products reorder with cheapest first |
| 3 | Verify the first product has the lowest price | Price order is ascending |

---

### TC-PL-007 — Sort products by price (high to low)
**Priority:** High
**Preconditions:** User is signed in, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Select "Price: High to Low" from sort control | Products reorder with most expensive first |
| 2 | Verify the first product has the highest price | Price order is descending |

---

### TC-PL-008 — Home page redirects unauthenticated users appropriately
**Priority:** Medium
**Preconditions:** User is not signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/` | Page loads (home is publicly accessible) OR user is redirected to signin |
| 2 | Verify the experience is consistent | No errors or blank page |

---

## 3. Product Detail & Add to Cart

### TC-PC-001 — Add a single product to cart
**Priority:** High
**Preconditions:** User is signed in as `demouser`, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Locate any product on the listing | Product card visible with an "Add to Cart" button |
| 2 | Click **Add to Cart** on the product | Cart icon/count in the header increments by 1 |
| 3 | Verify product is added | Cart count shows 1 |

---

### TC-PC-002 — Add multiple different products to cart
**Priority:** High
**Preconditions:** User is signed in, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Add to Cart** on Product A | Cart count shows 1 |
| 2 | Click **Add to Cart** on Product B | Cart count shows 2 |
| 3 | Navigate to cart/checkout | Both products appear as separate line items |

---

### TC-PC-003 — Add the same product to cart multiple times
**Priority:** Medium
**Preconditions:** User is signed in, on the home page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Add to Cart** on Product A | Cart count shows 1 |
| 2 | Click **Add to Cart** on Product A again | Cart count increases (either quantity increments or a second line item appears) |
| 3 | Navigate to checkout | Product A quantity or line item count reflects multiple additions |

---

### TC-PC-004 — Add to Favourites from product listing
**Priority:** Medium
**Preconditions:** User is signed in as `demouser`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Locate the favourite/wishlist icon on a product card | Icon is visible |
| 2 | Click the favourite icon | Icon changes state (e.g. fills in) to indicate the product is favourited |
| 3 | Navigate to `/favourites` | The product appears in the favourites list |

---

### TC-PC-005 — Remove a product from favourites
**Priority:** Medium
**Preconditions:** User is signed in, at least one product is in favourites

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/favourites` | Favourited products are listed |
| 2 | Click the favourite icon on a product to unfavourite it | Product is removed from the favourites list |
| 3 | Verify the list updates | Removed product no longer appears |

---

## 4. Favourites

### TC-FA-001 — Favourites page requires authentication
**Priority:** High
**Preconditions:** User is not signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/favourites` | User is redirected to `/signin?favourites=true` |
| 2 | Sign in with `demouser` | User is returned to `/favourites` |

---

### TC-FA-002 — fav_user sees pre-seeded favourites
**Priority:** High
**Preconditions:** Signed in as `fav_user`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/favourites` | Page loads with pre-existing favourited products |
| 2 | Verify products are displayed | At least one product is shown in the list |
| 3 | Verify product details are correct | Each item shows image, name, and price |

---

### TC-FA-003 — Empty favourites state for new user
**Priority:** Medium
**Preconditions:** Signed in as `demouser` with no products favourited

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/favourites` | Page loads without errors |
| 2 | Verify the empty state | An appropriate empty state message or illustration is shown |
| 3 | Verify no product cards are rendered | The list is empty |

---

### TC-FA-004 — Add to cart from favourites page
**Priority:** Medium
**Preconditions:** Signed in as `fav_user`, products exist in favourites

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/favourites` | Favourited products visible |
| 2 | Click **Add to Cart** on a favourited product | Cart count in header increments |
| 3 | Navigate to checkout | The product appears in the checkout summary |

---

## 5. Cart

### TC-CA-001 — Cart persists items added from product listing
**Priority:** High
**Preconditions:** User is signed in, has added items to cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Add one product to the cart from the home page | Cart count shows 1 |
| 2 | Navigate away to `/favourites` | Page loads |
| 3 | Navigate back to the home page | Cart count still shows 1 |

---

### TC-CA-002 — Cart is cleared after successful checkout
**Priority:** High
**Preconditions:** User is signed in, has completed a full checkout

| Step | Action | Expected Result |
|---|---|---|
| 1 | Add a product and complete checkout successfully | Confirmation page shown |
| 2 | Navigate back to the home page | Cart count shows 0 or is empty |

---

### TC-CA-003 — Cart is empty when no items added
**Priority:** Medium
**Preconditions:** User is signed in as `demouser`, no items added

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/checkout` | Empty cart state is shown OR user is redirected |
| 2 | Verify no items appear in the order summary | Summary shows 0 items / empty state message |

---

## 6. Checkout

### TC-CH-001 — Successful checkout with all required fields
**Priority:** High
**Preconditions:** User is signed in as `demouser`, at least one item in cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/checkout` | Checkout page loads with shipping form and order summary |
| 2 | Enter `John` in First Name | Field accepts input |
| 3 | Enter `Doe` in Last Name | Field accepts input |
| 4 | Enter `123 Main Street` in Address | Field accepts input |
| 5 | Enter `California` in State/Province | Field accepts input |
| 6 | Enter `90001` in Postal Code | Field accepts input |
| 7 | Click **Submit** | User is redirected to `/confirmation` |

---

### TC-CH-002 — Checkout blocked when First Name is empty
**Priority:** High
**Preconditions:** User is signed in, has items in cart, on `/checkout`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Leave First Name empty | Field is blank |
| 2 | Fill all other fields with valid data | Other fields populated |
| 3 | Click **Submit** | Form does not submit; validation error shown on First Name |

---

### TC-CH-003 — Checkout blocked when Last Name is empty
**Priority:** High
**Preconditions:** User on `/checkout` with items in cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Fill all fields except Last Name | Last Name is blank |
| 2 | Click **Submit** | Validation error shown; form not submitted |

---

### TC-CH-004 — Checkout blocked when Address is empty
**Priority:** High
**Preconditions:** User on `/checkout` with items in cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Fill all fields except Address | Address is blank |
| 2 | Click **Submit** | Validation error shown; form not submitted |

---

### TC-CH-005 — Checkout blocked when State/Province is empty
**Priority:** High
**Preconditions:** User on `/checkout` with items in cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Fill all fields except State/Province | State is blank |
| 2 | Click **Submit** | Validation error shown; form not submitted |

---

### TC-CH-006 — Checkout blocked when Postal Code is empty
**Priority:** High
**Preconditions:** User on `/checkout` with items in cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Fill all fields except Postal Code | Postal Code is blank |
| 2 | Click **Submit** | Validation error shown; form not submitted |

---

### TC-CH-007 — Checkout blocked when all fields are empty
**Priority:** High
**Preconditions:** User on `/checkout` with items in cart

| Step | Action | Expected Result |
|---|---|---|
| 1 | Leave all shipping fields empty | All fields blank |
| 2 | Click **Submit** | Form does not submit; one or more validation errors displayed |

---

### TC-CH-008 — Order summary shows correct items and total
**Priority:** High
**Preconditions:** User has added 2 products to cart and is on `/checkout`

| Step | Action | Expected Result |
|---|---|---|
| 1 | View the Order Summary section | Both added products are listed |
| 2 | Verify product title, size, and quantity per line item | All match what was added |
| 3 | Verify the Total (USD) is correct | Total equals sum of (price × quantity) for all items |

---

### TC-CH-009 — Checkout requires authentication
**Priority:** High
**Preconditions:** User is not signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/checkout` | User is redirected to `/signin?checkout=true` |

---

### TC-CH-010 — Checkout with empty cart redirects or shows empty state
**Priority:** Medium
**Preconditions:** User is signed in, cart is empty

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/checkout` | Empty state is displayed OR user is redirected to home |
| 2 | Verify the Submit button is not accessible when cart is empty | Button is disabled or not shown |

---

## 7. Order Confirmation

### TC-CF-001 — Confirmation page displays after successful checkout
**Priority:** High
**Preconditions:** User has just completed a checkout

| Step | Action | Expected Result |
|---|---|---|
| 1 | Submit a valid checkout form | User is redirected to `/confirmation` |
| 2 | Verify the heading message | "Your Order has been successfully placed." is displayed |
| 3 | Verify an order number is shown | A numeric order number (1–100) is visible |
| 4 | Verify the Order Summary is displayed | Products, quantities, and total shown correctly |

---

### TC-CF-002 — Continue Shopping button returns to home
**Priority:** High
**Preconditions:** User is on the confirmation page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Continue Shopping »** | User is redirected to the home page `/` |
| 2 | Verify the cart is now empty | Cart count shows 0 |

---

### TC-CF-003 — Download order receipt generates a PDF
**Priority:** Medium
**Preconditions:** User is on the confirmation page

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click **Download order receipt** | A PDF file is downloaded |
| 2 | Open the downloaded PDF | PDF contains "BrowserStack Demo" as header |
| 3 | Verify PDF contents | Table lists Brand, Type, Quantity, Price for each item |
| 4 | Verify the total price in the PDF | Matches the total shown on the confirmation page |

---

### TC-CF-004 — Confirmation page cannot be accessed directly without placing an order
**Priority:** Medium
**Preconditions:** User is signed in but has not placed an order in this session

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate directly to `/confirmation` | User is redirected to home OR an empty/error state is shown |
| 2 | Verify no fake order data is displayed | No order summary appears |

---

### TC-CF-005 — Order number is within valid range
**Priority:** Low
**Preconditions:** User has just placed an order

| Step | Action | Expected Result |
|---|---|---|
| 1 | View the order number on the confirmation page | A number is displayed |
| 2 | Verify the number is between 1 and 100 | Number is a positive integer in the range 1–100 |

---

## 8. Orders

### TC-OR-001 — Orders page requires authentication
**Priority:** High
**Preconditions:** User is not signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/orders` | User is redirected to `/signin?orders=true` |
| 2 | Sign in with `demouser` | User is redirected back to `/orders` |

---

### TC-OR-002 — existing_orders_user sees order history
**Priority:** High
**Preconditions:** Signed in as `existing_orders_user`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/orders` | Page loads with pre-existing orders |
| 2 | Verify at least one order is shown | Order card(s) are visible |
| 3 | Verify each order card shows: order placed date, total, ship-to name | All three fields present |
| 4 | Verify delivery status is displayed | "Delivered" label with a date shown |

---

### TC-OR-003 — Orders page shows empty state for user with no orders
**Priority:** Medium
**Preconditions:** Signed in as `demouser` (no pre-seeded orders)

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/orders` | Page loads without errors |
| 2 | Verify the empty state message | "No orders found" is displayed |
| 3 | Verify no order cards are rendered | The list is empty |

---

### TC-OR-004 — Order placed via checkout appears in order history
**Priority:** High
**Preconditions:** User is signed in as `demouser` and has just placed an order

| Step | Action | Expected Result |
|---|---|---|
| 1 | Complete a full checkout flow | Confirmation page shown |
| 2 | Navigate to `/orders` | The new order appears in the list |
| 3 | Verify the order details match what was checked out | Total and items are correct |

---

### TC-OR-005 — Order details display correct product information
**Priority:** Medium
**Preconditions:** Signed in as `existing_orders_user`, orders are visible

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/orders` | Orders are listed |
| 2 | Locate an order and verify item details | Each item shows: product image, title, description, price |
| 3 | Verify prices are formatted as currency | Prices display with correct decimal formatting |

---

## 9. Offers

### TC-OF-001 — Offers page requires authentication
**Priority:** High
**Preconditions:** User is not signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` | User is redirected to `/signin?offers=true` |
| 2 | Sign in | User is returned to `/offers` |

---

### TC-OF-002 — Offers page requests geolocation permission
**Priority:** High
**Preconditions:** User is signed in, on `/offers`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` | Browser displays a geolocation permission prompt |
| 2 | Verify the prompt is triggered by the page | Prompt appears before any offers are loaded |

---

### TC-OF-003 — Offers display when geolocation is allowed
**Priority:** High
**Preconditions:** User is signed in, browser geolocation is enabled

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` and allow geolocation | Location permission granted |
| 2 | Wait for page to load offers | Offers are fetched from `/api/offers` |
| 3 | Verify offers are displayed | At least one offer card shown with image and title |

---

### TC-OF-004 — Error message when geolocation is denied
**Priority:** High
**Preconditions:** User is signed in, browser geolocation is blocked

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` and deny geolocation | Permission denied |
| 2 | Verify the error message | "Please enable Geolocation in your browser." is displayed |
| 3 | Verify no offer cards are shown | The offers grid is empty |

---

### TC-OF-005 — No offers available for current location
**Priority:** Medium
**Preconditions:** User is signed in, geolocation allowed, but no offers exist for that region

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` with geolocation allowed | Location retrieved |
| 2 | Wait for API response | No offers returned |
| 3 | Verify the message | "Sorry we do not have any promotional offers in your city." is shown |

---

### TC-OF-006 — Offers page in a browser without geolocation support
**Priority:** Low
**Preconditions:** User is signed in, using an older browser without geolocation API

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` | Page loads |
| 2 | Verify the browser compatibility message | "Geolocation is not available in your browser." is displayed |

---

### TC-OF-007 — Each offer card displays image and title
**Priority:** Medium
**Preconditions:** User is signed in, geolocation allowed, offers are available

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/offers` and allow geolocation | Offers load |
| 2 | Inspect each offer card | Each card shows an image (150px height) and a title below it |
| 3 | Verify no cards have missing images or blank titles | All offers are fully rendered |

---

## 10. Navigation & Header

### TC-NA-001 — Header navigation links are accessible when signed in
**Priority:** High
**Preconditions:** User is signed in as `demouser`

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to the home page | Header is visible |
| 2 | Verify navigation links present | Links to Home, Favourites, Orders, Offers, and Sign Out (or similar) are shown |
| 3 | Click each navigation link | Each link navigates to the correct page |

---

### TC-NA-002 — Sign out clears session and redirects to sign in
**Priority:** High
**Preconditions:** User is signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Click the Sign Out link/button in the header | User is redirected to `/signin` |
| 2 | Attempt to navigate to `/orders` | User is redirected back to signin (session is cleared) |
| 3 | Attempt to navigate to `/checkout` | User is redirected to signin |

---

### TC-NA-003 — Cart count in header reflects items added
**Priority:** High
**Preconditions:** User is signed in, cart is empty

| Step | Action | Expected Result |
|---|---|---|
| 1 | Verify cart count shows 0 (or is not shown) | Cart is empty |
| 2 | Add one product to cart | Cart count increments to 1 |
| 3 | Add another product | Cart count increments to 2 |
| 4 | Complete checkout | Cart count resets to 0 |

---

### TC-NA-004 — StackDemo logo navigates to home
**Priority:** Low
**Preconditions:** User is on any page other than home

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/orders` | Orders page displayed |
| 2 | Click the StackDemo logo in the header | User is redirected to `/` |

---

### TC-NA-005 — Footer is present on all pages
**Priority:** Low
**Preconditions:** User is signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Navigate to `/` | Footer is visible at the bottom |
| 2 | Navigate to `/orders` | Footer is visible |
| 3 | Navigate to `/checkout` | Footer is visible |
| 4 | Navigate to `/favourites` | Footer is visible |

---

*Total test cases: 55*

---

## 11. Exploratory Testing

> **Session date:** 2026-02-20
> **Method:** Playwright headless automation + manual observation
> **Scope:** Edge cases, accessibility, data integrity, navigation bugs

---

### TC-EX-001 — "Sign In" nav link has incorrect href (`/offers` instead of `/signin`)
**Priority:** High
**Bug:** The "Sign In" link in the header points to `/offers` via its `href` attribute. Because unauthenticated users are redirected from `/offers` to `/signin?offers=true`, the link appears to work, but it sets an unintended `?offers=true` query parameter and causes an unnecessary redirect hop.

| Step | Action | Expected Result |
|---|---|---|
| 1 | Open the site as a guest (not signed in) | Home page loads |
| 2 | Inspect the "Sign In" link in the header | `href` attribute should be `/signin` |
| 3 | Click the "Sign In" link | User navigates directly to `/signin` (no detour through `/offers`) |
| 4 | Verify the URL | URL is `/signin` with no `?offers=true` query parameter |

**Actual (Bug):** `href="/offers"` — user is redirected through offers, landing on `/signin?offers=true`.

---

### TC-EX-002 — Logout redirects to home page instead of sign-in page
**Priority:** Medium
**Bug:** After clicking Logout, users are redirected to `/` (home) instead of `/signin`, leaving them on a page where they see all product listings but cannot access protected features.

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as `demouser` | User is authenticated |
| 2 | Click **Logout** in the header | User is redirected to `/signin` |
| 3 | Verify the URL | URL is `/signin` |
| 4 | Verify session is cleared | Header no longer shows username; sign-in form is displayed |

**Actual (Bug):** Logout redirects to `/` (home page), not `/signin`.

---

### TC-EX-003 — Order history shows incorrect item prices ($10.00 for all items)
**Priority:** High
**Bug:** On the `/orders` page for `existing_orders_user`, every individual line item is priced at $10.00 regardless of the actual product price (e.g. iPhone 12 retails at $799 on the home page).

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as `existing_orders_user` | Authenticated |
| 2 | Navigate to `/orders` | Order history is displayed |
| 3 | Expand an order containing iPhone 12 | Line item for iPhone 12 should show $799.00 |
| 4 | Check all visible item prices | Each item price matches the corresponding product's actual retail price |
| 5 | Verify the order total is a sum of item prices × quantities | Total = sum of (price × qty) |

**Actual (Bug):** All item prices show as $10.00. Order total shows $125 which does not correspond to any correct sum.

---

### TC-EX-004 — Favourites heart button has incorrect `aria-label` ("delete")
**Priority:** Medium
**Bug:** The heart/favourite icon button on every product card on the home page has `aria-label="delete"`. This is misleading to screen-reader users and represents an accessibility defect.

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as `demouser` | Home page loads with product grid |
| 2 | Inspect the heart icon button on any product card | `aria-label` should be something like "Add to favourites" or "Toggle favourite" |
| 3 | Verify the button communicates its purpose | The label accurately describes the action |

**Actual (Bug):** `aria-label="delete"` on all product favourite buttons.

---

### TC-EX-005 — Unauthenticated users can add products to cart
**Priority:** Low
**Note:** Observe-and-document. This may be intentional (guest cart), but authentication is required at checkout.

| Step | Action | Expected Result |
|---|---|---|
| 1 | Open the site without signing in | Home page loads with 25 products |
| 2 | Click **Add to cart** on any product | Expected: prompt to sign in OR cart increments (if guest cart is supported) |
| 3 | Verify cart badge increments | Cart badge shows 1 |
| 4 | Navigate to `/checkout` | User is redirected to `/signin?checkout=true` |
| 5 | Sign in | User is returned to `/checkout` with cart items still present |

**Actual:** Cart badge increments for guests (float cart opens and shows the item). Navigation to `/checkout` correctly requires authentication. Cart items persist after sign-in.

---

### TC-EX-006 — Checkout form has no custom validation error messages
**Priority:** Medium
**Bug:** When submitting the checkout form with empty required fields, only the browser's built-in HTML5 native validation tooltip is shown ("Please fill out this field."). There are no custom styled error messages or highlighted fields in the UI.

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in and add an item to cart | Item in cart |
| 2 | Navigate to `/checkout` | Checkout page loads |
| 3 | Leave all fields empty and click **Submit** | Custom inline error messages appear below each empty required field, styled to match the app design |
| 4 | Verify error state styling | Required fields are visually highlighted (e.g. red border, error icon) |

**Actual (Bug):** Only browser-native HTML5 validation tooltip appears. No custom error UI elements or field highlighting.

---

### TC-EX-007 — Product listing shows installment pricing alongside main price
**Priority:** Low
**Note:** Informational / verify correctness.

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in and view the home page | Product cards are visible |
| 2 | For each product, note the main price and the installment price (e.g. "or 9 x $88.78") | Installment calculation should be: main price ÷ number of installments = installment amount |
| 3 | Verify iPhone 12 ($799.00): 9 × $88.78 = $799.02 | Rounding difference of $0.02 — acceptable OR exact match expected? |
| 4 | Verify installment counts are consistent (some show 9×, others 5×, 3×) | Document whether different installment terms are intentional per product |

**Actual:** Installment amounts are mathematically correct (within rounding). Installment counts vary per product (3×, 5×, 6×, 7×, 8×, 9×, 12×).

---

### TC-EX-008 — Accessing `/confirmation` directly without an order redirects to home
**Priority:** Low
**Preconditions:** User is signed in but has NOT placed an order in the current session

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as `demouser` | Authenticated |
| 2 | Navigate directly to `/confirmation` | Expected: redirect to home OR a clear "No order found" message |
| 3 | Verify no fake order data is displayed | No order number, no summary, no receipt link |

**Actual:** Redirects to `/` (home page). No fake data is shown — correct behaviour confirmed.

---

### TC-EX-009 — Already authenticated user navigating to `/signin` is redirected to home
**Priority:** Low
**Preconditions:** User is already signed in

| Step | Action | Expected Result |
|---|---|---|
| 1 | Sign in as `demouser` | URL becomes `/?signin=true` |
| 2 | Navigate directly to `/signin` | User should be redirected away (e.g. to `/`) since they are already authenticated |
| 3 | Verify the sign-in form is NOT shown | Home page (or another appropriate page) loads instead |

**Actual:** Correctly redirects to `/` when already signed in. Consistent behaviour confirmed.

---

*Total exploratory test cases: 9 (TC-EX-001 to TC-EX-009)*
*Confirmed bugs: TC-EX-001, TC-EX-002, TC-EX-003, TC-EX-004, TC-EX-006*
