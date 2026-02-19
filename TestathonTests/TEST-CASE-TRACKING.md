# Test Case Creation Tracking
> Project: StackDemo — https://testathon.live
> Last updated: 2026-02-20

---

## Summary

| Section | File | Tests | Status |
|---|---|---|---|
| Sign In | `SignInTests.cs` | TC-SI-001 to TC-SI-010 | ✅ Complete |
| Home & Product Listing | `ProductListingTests.cs` | TC-PL-001 to TC-PL-008 | ✅ Complete |
| Product Detail & Add to Cart | `ProductCartTests.cs` | TC-PC-001 to TC-PC-005 | ✅ Complete |
| Favourites | `FavouritesTests.cs` | TC-FA-001 to TC-FA-004 | ✅ Complete |
| Cart | `CartTests.cs` | TC-CA-001 to TC-CA-003 | ✅ Complete |
| Checkout | `CheckoutTests.cs` | TC-CH-001 to TC-CH-010 | ✅ Complete |
| Order Confirmation | `OrderConfirmationTests.cs` | TC-CF-001 to TC-CF-005 | ✅ Complete |
| Orders | `OrdersTests.cs` | TC-OR-001 to TC-OR-005 | ✅ Complete |
| Offers | `OffersTests.cs` | TC-OF-001 to TC-OF-007 | ✅ Complete |
| Navigation & Header | `NavigationTests.cs` | TC-NA-001 to TC-NA-005 | ✅ Complete |
| Homepage (smoke) | `HomepageTests.cs` | TC-99 | ✅ Complete (pre-existing) |

**Total: 55 test cases across 11 files — all complete**

---

## Detailed Status

### ✅ Sign In — `SignInTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-SI-001 | Successful sign in with valid credentials | ✅ Done | |
| TC-SI-002 | Sign in redirects to checkout from cart | ✅ Done | |
| TC-SI-003 | Sign in redirects to favourites | ✅ Done | |
| TC-SI-004 | Sign in redirects to offers | ✅ Done | |
| TC-SI-005 | Locked user cannot sign in | ✅ Done | |
| TC-SI-006 | Username dropdown shows all 5 options | ✅ Done | |
| TC-SI-007 | No direct text entry in username | ✅ Done | |
| TC-SI-008 | No credentials shows error | ✅ Done | |
| TC-SI-009 | Username but no password shows error | ✅ Done | |
| TC-SI-010 | Sign in page accessible; redirects when logged in | ✅ Done | |

---

### ✅ Home & Product Listing — `ProductListingTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-PL-001 | Home page loads with product listings | ✅ Done | |
| TC-PL-002 | Product images load for demouser | ✅ Done | Checks `naturalWidth > 0` |
| TC-PL-003 | Product images fail for image_not_loading_user | ✅ Done | Checks `naturalWidth == 0` |
| TC-PL-004 | Filter by single category | ✅ Done | Uses Apple checkbox |
| TC-PL-005 | Filter by multiple categories | ✅ Done | Uses Apple + Samsung |
| TC-PL-006 | Sort by price low to high | ⚠️ Skipped | Sort control not on live site — `[Ignore]` attribute applied |
| TC-PL-007 | Sort by price high to low | ⚠️ Skipped | Sort control not on live site — `[Ignore]` attribute applied |
| TC-PL-008 | Home page accessible without authentication | ✅ Done | |

---

### ✅ Product Detail & Add to Cart — `ProductCartTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-PC-001 | Add single product to cart | ✅ Done | Verifies `.bag__quantity` = 1 |
| TC-PC-002 | Add multiple different products | ✅ Done | Verifies count = 2 and 2 cart items |
| TC-PC-003 | Add same product multiple times | ✅ Done | Verifies count increases |
| TC-PC-004 | Add to favourites from listing | ✅ Done | Verifies `button.clicked` + /favourites |
| TC-PC-005 | Remove from favourites | ✅ Done | Uses fav_user pre-seeded data |

---

### ✅ Favourites — `FavouritesTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-FA-001 | Favourites requires authentication | ✅ Done | Verifies `/signin?favourites=true` redirect |
| TC-FA-002 | fav_user sees pre-seeded favourites | ✅ Done | Verifies image, title, price per item |
| TC-FA-003 | Empty favourites for new user | ✅ Done | Verifies "0 Product(s) found." |
| TC-FA-004 | Add to cart from favourites page | ✅ Done | Uses fav_user |

---

### ✅ Cart — `CartTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-CA-001 | Cart persists items across navigation | ✅ Done | Navigate to /favourites and back |
| TC-CA-002 | Cart cleared after successful checkout | ✅ Done | Verifies badge = 0 after checkout |
| TC-CA-003 | Empty cart at checkout | ✅ Done | Handles both empty state + no-form cases |

---

### ✅ Checkout — `CheckoutTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-CH-001 | Successful checkout | ✅ Done | Full flow → /confirmation |
| TC-CH-002 | Blocked when First Name empty | ✅ Done | Verifies stays on /checkout |
| TC-CH-003 | Blocked when Last Name empty | ✅ Done | |
| TC-CH-004 | Blocked when Address empty | ✅ Done | |
| TC-CH-005 | Blocked when State/Province empty | ✅ Done | |
| TC-CH-006 | Blocked when Postal Code empty | ✅ Done | |
| TC-CH-007 | Blocked when all fields empty | ✅ Done | |
| TC-CH-008 | Order summary accuracy | ✅ Done | Verifies 2 items + total |
| TC-CH-009 | Checkout requires authentication | ✅ Done | |
| TC-CH-010 | Empty cart at checkout | ✅ Done | |

---

### ✅ Order Confirmation — `OrderConfirmationTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-CF-001 | Confirmation page after checkout | ✅ Done | `legend#confirmation-message` |
| TC-CF-002 | Continue Shopping returns to home | ✅ Done | `button.button--tertiary` |
| TC-CF-003 | Download receipt generates PDF | ✅ Done | `a#downloadpdf`, uses `RunAndWaitForDownloadAsync` |
| TC-CF-004 | Direct /confirmation access blocked | ✅ Done | Handles redirect or empty state |
| TC-CF-005 | Order number in valid range 1–100 | ✅ Done | Parses strong element text |

---

### ✅ Orders — `OrdersTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-OR-001 | Orders requires authentication | ✅ Done | `/signin?orders=true` redirect |
| TC-OR-002 | existing_orders_user sees order history | ✅ Done | Verifies date, total, ship-to, delivered |
| TC-OR-003 | Empty state for user with no orders | ✅ Done | "No orders found" |
| TC-OR-004 | New order appears after checkout | ✅ Done | Uses demouser checkout flow |
| TC-OR-005 | Order details correct | ✅ Done | image, title, price format |

---

### ✅ Offers — `OffersTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-OF-001 | Offers requires authentication | ✅ Done | `/signin?offers=true` redirect |
| TC-OF-002 | Offers requests geolocation | ✅ Done | Verifies geo-related content shown |
| TC-OF-003 | Offers display when geolocation allowed | ✅ Done | Uses `Context.SetGeolocationAsync` |
| TC-OF-004 | Error when geolocation denied | ✅ Done | Default context = denied |
| TC-OF-005 | No offers for current location | ✅ Done | Remote coordinates used |
| TC-OF-006 | Browser without geolocation support | ✅ Done | `AddInitScriptAsync` override |
| TC-OF-007 | Each offer card has image + title | ✅ Done | Checks `height: 150px` on image |

---

### ✅ Navigation & Header — `NavigationTests.cs`
| ID | Name | Status | Notes |
|---|---|---|---|
| TC-NA-001 | Nav links accessible when signed in | ✅ Done | Clicks each link and verifies URL |
| TC-NA-002 | Sign out clears session | ✅ Done | `a#logout`, then checks /orders + /checkout redirect |
| TC-NA-003 | Cart count reflects items + resets | ✅ Done | Full add → checkout → verify 0 flow |
| TC-NA-004 | Logo navigates to home | ✅ Done | `a.Navbar_logo__26S5Y` |
| TC-NA-005 | Footer on all pages | ✅ Done | Checks `footer` on /, /orders, /checkout, /favourites |

---

## Known Limitations

| Issue | Affected Tests | Reason |
|---|---|---|
| Sort control not present | TC-PL-006, TC-PL-007 | The sort/order feature is not implemented on the live testathon.live site. Tests are written but marked `[Ignore]`. |
| Checkout validation errors | TC-CH-002 to TC-CH-007 | The form uses HTML5 native `required` attribute — no custom JS error elements. Tests verify the page stays on `/checkout`. |
| Confirmation order number selector | TC-CF-001, TC-CF-005 | Order number is in a `<strong>` element adjacent to `legend#confirmation-message`. |
