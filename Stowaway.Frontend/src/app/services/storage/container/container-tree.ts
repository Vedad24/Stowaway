import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ContainerApiService } from './container';

export interface ContainerOption {
  id: number;
  label: string;
}

@Injectable({
  providedIn: 'root',
})
export class ContainerTreeService {
  private readonly containerService = inject(ContainerApiService);

  // Containers only return their direct children, so the full "place" list is built
  // by walking the tree from the warehouse root and labeling each option with its
  // path, letting containers with the same name at different depths stay
  // distinguishable. `excludeContainerId`, when given, skips that container and its
  // entire subtree (used when picking a destination while deleting a container).
  async loadOptions(warehouseId: number, excludeContainerId?: number): Promise<ContainerOption[]> {
    const options: ContainerOption[] = [];

    const walk = async (parentContainerId: number | null, pathLabel: string): Promise<void> => {
      const children = await firstValueFrom(this.containerService.list({ warehouseId, parentContainerId }));
      for (const child of children) {
        if (child.id === excludeContainerId) {
          continue;
        }
        const label = pathLabel ? `${pathLabel} / ${child.name}` : child.name;
        options.push({ id: child.id, label });
        if (child.hasChildren) {
          await walk(child.id, label);
        }
      }
    };

    await walk(null, '');
    return options;
  }
}
