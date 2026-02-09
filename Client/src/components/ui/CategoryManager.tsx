import { useState } from "react";
import { IoAdd, IoTrash, IoChevronDown, IoChevronUp } from "react-icons/io5";

interface Item {
    id: string;
    name: string;
}

interface Category {
    id: string;
    name: string;
    items: Item[];
    isExpanded: boolean;
}

function CategoryManager() {
    const [categories, setCategories] = useState<Category[]>([
        {
            id: "1",
            name: "Electrónicos",
            isExpanded: true,
            items: [
                { id: "1-1", name: "Laptops" },
                { id: "1-2", name: "Celulares" },
                { id: "1-3", name: "PC Torres" },
            ],
        },
    ]);

    const [newCategoryName, setNewCategoryName] = useState("");
    const [newItemNames, setNewItemNames] = useState<{ [key: string]: string }>(
        {}
    );

    // Toggle category expansion
    const toggleCategory = (categoryId: string) => {
        setCategories(
            categories.map((cat) =>
                cat.id === categoryId ? { ...cat, isExpanded: !cat.isExpanded } : cat
            )
        );
    };

    // Add new category
    const addCategory = () => {
        if (newCategoryName.trim()) {
            const newCategory: Category = {
                id: Date.now().toString(),
                name: newCategoryName,
                items: [],
                isExpanded: true,
            };
            setCategories([...categories, newCategory]);
            setNewCategoryName("");
        }
    };

    // Delete category
    const deleteCategory = (categoryId: string) => {
        setCategories(categories.filter((cat) => cat.id !== categoryId));
    };

    // Add item to category
    const addItem = (categoryId: string) => {
        const itemName = newItemNames[categoryId];
        if (itemName && itemName.trim()) {
            setCategories(
                categories.map((cat) => {
                    if (cat.id === categoryId) {
                        const newItem: Item = {
                            id: `${categoryId}-${Date.now()}`,
                            name: itemName,
                        };
                        return {
                            ...cat,
                            items: [...cat.items, newItem],
                        };
                    }
                    return cat;
                })
            );
            setNewItemNames({ ...newItemNames, [categoryId]: "" });
        }
    };

    // Delete item from category
    const deleteItem = (categoryId: string, itemId: string) => {
        setCategories(
            categories.map((cat) => {
                if (cat.id === categoryId) {
                    return {
                        ...cat,
                        items: cat.items.filter((item) => item.id !== itemId),
                    };
                }
                return cat;
            })
        );
    };

    return (
        <div className="space-y-4">
            {/* Categories List */}
            {categories.map((category) => (
                <div
                    key={category.id}
                    className="rounded-lg border border-brand-200 bg-brand-50 shadow-sm transition-all duration-200 hover:shadow-md"
                >
                    {/* Category Header */}
                    <div className="flex items-center justify-between p-4">
                        <button
                            onClick={() => toggleCategory(category.id)}
                            className="flex flex-1 items-center gap-2 text-left font-semibold text-brand-900 transition-colors hover:text-brand-700"
                        >
                            {category.isExpanded ? (
                                <IoChevronUp className="h-5 w-5" />
                            ) : (
                                <IoChevronDown className="h-5 w-5" />
                            )}
                            <span>{category.name}</span>
                            <span className="ml-2 rounded-full bg-brand-200 px-2 py-0.5 text-xs text-brand-700">
                                {category.items.length}
                            </span>
                        </button>
                        <button
                            onClick={() => deleteCategory(category.id)}
                            className="ml-2 rounded-full p-2 text-red-500 transition-all hover:bg-red-50 hover:scale-110"
                            aria-label="Eliminar categoría"
                        >
                            <IoTrash className="h-4 w-4" />
                        </button>
                    </div>

                    {/* Category Items */}
                    {category.isExpanded && (
                        <div className="border-t border-brand-200 bg-white p-4">
                            <ul className="space-y-2">
                                {category.items.map((item) => (
                                    <li
                                        key={item.id}
                                        className="group flex items-center justify-between rounded-md bg-brand-50 px-3 py-2 transition-all hover:bg-brand-100"
                                    >
                                        <span className="text-sm text-brand-800">{item.name}</span>
                                        <button
                                            onClick={() => deleteItem(category.id, item.id)}
                                            className="opacity-0 rounded-full p-1 text-red-500 transition-all group-hover:opacity-100 hover:bg-red-50"
                                            aria-label="Eliminar item"
                                        >
                                            <IoTrash className="h-3.5 w-3.5" />
                                        </button>
                                    </li>
                                ))}
                            </ul>

                            {/* Add Item Input */}
                            <div className="mt-4 flex flex-col sm:flex-row gap-2">
                                <input
                                    type="text"
                                    value={newItemNames[category.id] || ""}
                                    onChange={(e) =>
                                        setNewItemNames({
                                            ...newItemNames,
                                            [category.id]: e.target.value,
                                        })
                                    }
                                    onKeyPress={(e) => {
                                        if (e.key === "Enter") {
                                            addItem(category.id);
                                        }
                                    }}
                                    placeholder="Nombre del item..."
                                    className="flex-1 min-w-0 rounded-lg border border-brand-300 px-3 py-2 text-sm transition-all focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/20"
                                />
                                <button
                                    onClick={() => addItem(category.id)}
                                    className="flex items-center justify-center gap-1 rounded-lg bg-brand-500 px-4 py-2 text-sm font-medium text-white transition-all hover:bg-brand-600 hover:scale-105 whitespace-nowrap min-w-[100px] sm:min-w-0"
                                >
                                    <IoAdd className="h-4 w-4 flex-shrink-0" />
                                    <span>Agregar</span>
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            ))}

            {/* Add Category Section */}
            <div className="rounded-lg border-2 border-dashed border-brand-300 bg-brand-50/50 p-4">
                <h3 className="mb-3 text-sm font-semibold text-brand-800">
                    Nueva Categoría
                </h3>
                <div className="flex flex-col sm:flex-row gap-2">
                    <input
                        type="text"
                        value={newCategoryName}
                        onChange={(e) => setNewCategoryName(e.target.value)}
                        onKeyPress={(e) => {
                            if (e.key === "Enter") {
                                addCategory();
                            }
                        }}
                        placeholder="Nombre de la categoría..."
                        className="flex-1 min-w-0 rounded-lg border border-brand-300 px-3 py-2 text-sm transition-all focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/20"
                    />
                    <button
                        onClick={addCategory}
                        className="flex items-center justify-center gap-1 rounded-lg bg-secondary-500 px-4 py-2 text-sm font-medium text-white transition-all hover:bg-secondary-600 hover:scale-105 whitespace-nowrap min-w-[100px] sm:min-w-0"
                    >
                        <IoAdd className="h-4 w-4 flex-shrink-0" />
                        <span>Agregar</span>
                    </button>
                </div>
            </div>
        </div>
    );
}

export default CategoryManager;
