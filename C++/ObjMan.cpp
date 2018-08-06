#include <string>
#include "ObjMan.h"

namespace obj {
	extern "C" {

		#pragma pack(2)
		struct StringDataCpp
		{
			char **pKeys;
			int *valueCounts;
		};

		typedef intptr_t ItemListHandle;

		Model model;

		float TestMult(float a, float b) {
			return a * b;
		}

		Model * Create(char* str) {
			model = obj::loadModelFromString(str);

			return &model;
		}

		int GetModelVertexCount(int modelPointer) {
			return model.vertex.size();
		}
		int GetModelTexCoordCount(int modelPointer) {
			return model.texCoord.size();
		}
		int GetModelNormalCount(int modelPointer) {
			return model.normal.size();
		}
		int GetModelFaceCount(int modelPointer) {
			return model.faces.size();
		}

		bool GetVertices(ItemListHandle* hItems, float** itemsFound, int* itemCount) {

			auto items = &model.vertex;

			*hItems = reinterpret_cast<ItemListHandle>(items);
			*itemsFound = items->data();
			*itemCount = items->size();

			return true;
		}
		bool GetNormals(ItemListHandle* hItems, float** itemsFound, int* itemCount) {

			auto items = &model.normal;

			*hItems = reinterpret_cast<ItemListHandle>(items);
			*itemsFound = items->data();
			*itemCount = items->size();

			return true;
		}

		bool GetTextureCoordinates(ItemListHandle* hItems, float** itemsFound, int* itemCount) {

			auto items = &model.texCoord;

			*hItems = reinterpret_cast<ItemListHandle>(items);
			*itemsFound = items->data();
			*itemCount = items->size();

			return true;
		}

		void GetFaceKeys(char *mdlFaceInput) {

			struct StringDataCpp *pFace = reinterpret_cast<struct StringDataCpp*>(mdlFaceInput);

			std::vector<std::string> keys;

			for (std::map<std::string, std::vector<unsigned short>>::iterator it = model.faces.begin(); it != model.faces.end(); ++it) {
				keys.push_back(it->first);
			}

			for (size_t i = 0; i < keys.size(); ++i) {

				char* value = const_cast<char*>(keys[i].c_str());
				int valueLength = keys[i].length();

				pFace->pKeys[i] = (char*)malloc(sizeof(char) * valueLength);
				strncpy(pFace->pKeys[i], value, valueLength);

				pFace->valueCounts[i] = (int)malloc(sizeof(int));
				pFace->valueCounts[i] = model.faces.find(value)->second.size();
			}
		}

		bool GetFaceValues(ItemListHandle* hItems, unsigned short** itemsFound, char* key, int* itemCount) {
			auto items = &model.faces.find(key)->second;

			*hItems = reinterpret_cast<ItemListHandle>(items);
			*itemsFound = items->data();
			*itemCount = items->size();

			return true;
		}

		bool ReleaseItems(ItemListHandle hItems) {
			auto items = reinterpret_cast<std::vector<float>*>(hItems);
			delete items;

			return true;
		}
	}
}