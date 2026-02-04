import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createBrowserRouter, RouterProvider } from "react-router-dom";

import AppLayout from "@/components/ui/AppLayout";
import ErrorPage from "@/components/ui/ErrorPage";
import Account from "@/features/account/Account";
import Address from "@/features/address/Address";
import Cart from "@/features/cart/Cart";
import Category from "@/features/category/Category";
import HomePage from "@/features/homepage/HomePage";
import Product from "@/features/product/Product";
import Wishlist from "@/features/wishlist/Wishlist";
import { PATHS } from "@/routes/paths";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60 * 1000,
    },
  },
});

const router = createBrowserRouter([
  {
    path: PATHS.home,
    element: <AppLayout />,
    errorElement: <ErrorPage />,
    children: [
      { index: true, element: <HomePage /> },
      { path: PATHS.account.slice(1), element: <Account /> },
      { path: PATHS.address.slice(1), element: <Address /> },
      { path: PATHS.cart.slice(1), element: <Cart /> },
      { path: PATHS.productDetails.slice(1), element: <Product /> },
      { path: PATHS.productsByCategory.slice(1), element: <Category /> },
      { path: PATHS.wishlist.slice(1), element: <Wishlist /> },
    ],
  },
]);

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  );
}

export default App;
