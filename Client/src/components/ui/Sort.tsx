import type { ProductSortType } from "@/lib/utils";

interface SortProps {
  sort: ProductSortType;
  handleSetSort: (sortType: ProductSortType) => void;
}

const sortTypes: Array<{ name: string; value: ProductSortType }> = [
  { name: "Default", value: "default" },
  { name: "Price: Lowest First", value: "price-asc" },
  { name: "Price: Highest First", value: "price-desc" },
];

function Sort({ sort, handleSetSort }: SortProps) {
  return (
    <div>
      <span className="mr-2">Sort by:</span>
      <select
        value={sort}
        onChange={(e) => handleSetSort(e.target.value as ProductSortType)}
        className="p-2 pl-0 font-bold"
      >
        {sortTypes.map((sort) => (
          <option key={sort.value} value={sort.value}>
            {sort.name}
          </option>
        ))}
      </select>
    </div>
  );
}

export default Sort;
