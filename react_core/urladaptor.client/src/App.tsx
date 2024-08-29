import { DataManager, UrlAdaptor } from '@syncfusion/ej2-data';
import { GridComponent, ColumnsDirective, ColumnDirective, Sort, Inject, Filter, Edit, Toolbar, Page, CommandColumn } from '@syncfusion/ej2-react-grids';
import { TextBoxComponent } from '@syncfusion/ej2-react-inputs'
import './App.css';

function App() {

    let grid: undefined | any;
    const data = new DataManager({
        url: 'http://localhost:5154/api/Grid', // Replace your hosted link
        insertUrl: 'http://localhost:5154/api/Grid/Insert',
        updateUrl: 'http://localhost:5154/api/Grid/Update',
        removeUrl: 'http://localhost:5154/api/Grid/Remove',
        //crudUrl:'https://localhost:7018/api/grid/CrudUpdate', // perform all CRUD action at single request using crudURL
        //batchUrl:'https://localhost:7018/api/grid/BatchUpdate', // perform CRUD action using batchURL when enabling batch editing
        adaptor: new UrlAdaptor()
    });

    let doValidation = true;
    let orderID: any;
    let UpdatedValue: any;
    let rowData: any;
    let saveData: any;

    async function serverSideValidation(data: any) {
        try {
            const response = await fetch('http://localhost:5154/api/Grid/Validate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({ value: data }) // Adjust this to the field you need to validate
            });
            const result = await response.json();
            return result.result;
        } catch (error) {
            console.error('Validation failed', error);
            return false; // In case of error, treat it as validation failure
        }
    }

    function dialogTemplate(props: any) {
        return (<DialogFormTemplate {...props} />);
    }

    function DialogFormTemplate(props: any) {
        return (
            <div>
                <div className="form-row">
                    <div className="form-group col-md-6">
                        <div>
                            <TextBoxComponent
                                id='OrderID'
                                value={props.OrderID}
                                floatLabelType="Auto"
                                placeholder="OrderID"
                            />
                        </div>
                    </div>
                </div>
                <div className="form-row">
                    <div className="form-group col-md-6">
                        <div>
                            <TextBoxComponent
                                id='ShipName'
                                value={props.ShipCountry}
                                floatLabelType="Auto"
                                placeholder="Ship Country"
                            />
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    const editOptions = {
        allowAdding: true,
        allowDeleting: true,
        allowEditing: true,
        showDeleteConfirmDialog: true,
        mode: 'Dialog',
        template: dialogTemplate,
    };
    const toolbarOptions = ['Add', 'Edit', 'Delete', 'Update', 'Cancel', 'Search'];

    const commands: any = [
        { type: 'Edit', buttonOption: { cssClass: 'e-flat', iconCss: 'e-edit e-icons' } },
        { type: 'Delete', buttonOption: { cssClass: 'e-flat', iconCss: 'e-delete e-icons' } },
        { type: 'Save', buttonOption: { cssClass: 'e-flat', iconCss: 'e-update e-icons' } },
        { type: 'Cancel', buttonOption: { cssClass: 'e-flat', iconCss: 'e-cancel-icon e-icons' } }
    ];

    let isCheck = true;
    const actionBegin = async (args: any) => {
        /** cast string to integer value */
        if (args.requestType === 'delete' && isCheck) {
            args.cancel = true;
            // Perform server-side validation before saving
            const isValid: boolean = await serverSideValidation(args.data[0]);
            if(isValid){
                isCheck = false
                grid.editSettings.showDeleteConfirmDialog = false;
                grid.deleteRow(args.tr[0]);
                grid.editSettings.showDeleteConfirmDialog = true;
                isCheck = true
            } 
        }
    };

    return <GridComponent
        ref={(g) => (grid = g)}
        dataSource={data}
        actionBegin={actionBegin}
        editSettings={editOptions}
        toolbar={toolbarOptions}
        height={265}
    >
        <ColumnsDirective>
            <ColumnDirective
                field="OrderID"
                headerText="Order ID"
                width="100"
                textAlign="Right"
                isPrimaryKey={true}
            />
            <ColumnDirective
                field="CustomerID"
                headerText="Customer ID"
                width="120"
            />
            <ColumnDirective
                field="ShipCountry"
                headerText="Ship Country"
                width="150"
            />
            <ColumnDirective headerText='Commands' width='120' commands={commands}/>
        </ColumnsDirective>
        <Inject services={[Edit, Toolbar, CommandColumn]} />
    </GridComponent>

}
export default App;