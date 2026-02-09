import { useState } from "react";
import { IoMenu, IoClose } from "react-icons/io5";
import CategoryManager from "./CategoryManager";

function BurgerMenu() {
    const [isOpen, setIsOpen] = useState(false);

    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };

    return (
        <>
            {/* Burger Button - Only shows when menu is closed */}
            {!isOpen && (
                <button
                    onClick={toggleMenu}
                    className="fixed left-4 top-20 z-[1000] flex h-12 w-12 items-center justify-center rounded-full bg-brand-500 text-white shadow-lg transition-all duration-300 hover:bg-brand-600 hover:scale-110 md:left-6 md:top-24 lg:top-28"
                    aria-label="Open menu"
                >
                    <IoMenu className="h-6 w-6" />
                </button>
            )}

            {/* Overlay */}
            {isOpen && (
                <div
                    className="fixed inset-0 z-[1000] bg-black/50 backdrop-blur-sm transition-opacity duration-300"
                    onClick={toggleMenu}
                />
            )}

            {/* Sidebar Menu */}
            <div
                className={`fixed left-0 top-0 pt-0 z-[1001] h-full w-full sm:w-96 md:w-[28rem] lg:w-[32rem] transform bg-white shadow-2xl transition-transform duration-300 ease-in-out overflow-hidden ${isOpen ? "translate-x-0" : "-translate-x-full"
                    }`}
            >
                <div className="flex h-full flex-col">
                    {/* Header with Close Button */}
                    <div className="flex items-center justify-between border-b border-brand-200 p-4 sm:p-6 bg-white">
                        <h2 className="text-lg sm:text-xl font-bold text-brand-900 flex-1">
                            Gestión de Categorías
                        </h2>
                        <button
                            onClick={toggleMenu}
                            className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-full bg-brand-500 text-white transition-all duration-300 hover:bg-brand-600 hover:scale-110 ml-2 shadow-md"
                            aria-label="Close menu"
                        >
                            <IoClose className="h-6 w-6" />
                        </button>
                    </div>

                    {/* Content with proper padding */}
                    <div className="flex-1 overflow-y-auto p-4 sm:p-6">
                        <CategoryManager />
                    </div>
                </div>
            </div>
        </>
    );
}

export default BurgerMenu;
